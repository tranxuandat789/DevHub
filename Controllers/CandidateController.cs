//03/06/2026 DatTX
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using DevHub.ViewModels.Candidate;

namespace DevHub.Controllers
{
    public class CandidateController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;
        private readonly ICandidateService _candidateService;
        private readonly ICvService _cvService;

        public CandidateController(IAuthService authService, IWebHostEnvironment env, ICandidateService candidateService, ICvService cvService)
        {
            _authService = authService;
            _env = env;
            _candidateService = candidateService;
            _cvService = cvService;
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult Dashboard()
        {
            var model = new DevHub.Models.DashboardViewModel
            {
                AppliedJobsCount = 24,
                SavedJobsCount = 15,
                InterviewsCount = 3
            };
            return View("~/Views/Candidate/CandidateDashboard/Index.cshtml", model);
        }
        // View profile
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> Profile()
        {
            // take user id from claim cookie
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var candidate = await _candidateService.GetCandidateByIdAsync(userId);
            var cv = await _cvService.GetCvByCandidateIdAsync(userId);
            if (candidate == null) { 
                return NotFound("Ứng viên không tồn tại");
            }
            var viewModel = new CandidateProfileViewModel
            {
                CandidateInfo = candidate,
                Cv = cv,
                ProfileCompletion = candidate.ProfileCompletion ?? 0
            };
            return View("~/Views/Candidate/CandidateProfile/Index.cshtml", viewModel);
        }
        // upload avatar
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar != null && avatar.Length > 0)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    return Json(new { success = false, message = "Không xác định được người dùng" });
                }

                try
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/avatars/{uniqueFileName}";
                    await _authService.SyncCandidateAvatarAsync(userId, relativePath);

                    return Json(new { success = true, message = "Cập nhật ảnh đại diện thành công", url = relativePath });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Không có tệp nào được chọn" });
        }
        // Update basic info
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UpdateBasicInfo(UpdateBasicInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                TempData["ErrorMessage"] = firstError ?? "Dữ liệu không hợp lệ!";
                return RedirectToAction("Profile");
            }
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int candidateId = int.Parse(userIdStr!);
                await _candidateService.UpdateProfileAsync(
                    candidateId, 
                    model.FullName, 
                    model.Phone, 
                    model.Birthdate, 
                    model.Gender, 
                    model.Address, 
                    model.SocialMediaUrl,
                    model.ExpectedSalaryMin,
                    model.ExpectedSalaryMax,
                    model.PreferredLocation,
                    model.ExperienceYears,
                    model.CvSearchable
                );

                TempData["SuccessMessage"] = "Cập nhật thông tin cơ bản thành công!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Profile");
        }

        // Upload CV file
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UploadCv(IFormFile cvFile)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int candidateId = int.Parse(userIdStr!);

                await _cvService.UploadCvFileAsync(candidateId, cvFile, _env.WebRootPath);

                await _candidateService.CalculateAndSaveCompletionAsync(candidateId);

                TempData["SuccessMessage"] = "Tải lên CV mới thành công! File cũ đã tự động bị xóa.";
            }
            catch (ArgumentException ex) 
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex) 
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lưu file: " + ex.Message;
            }

            return RedirectToAction("Profile");
        }
        // Update education, experience, skills, languages sections of CV
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UpdateEducation(string School, string Major, string Degree, string StartDate)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int candidateId = int.Parse(userIdStr!);
            var json = System.Text.Json.JsonSerializer.Serialize(new[] { new { School, Major, Degree, StartDate } });
            await _cvService.UpdateEducationAsync(candidateId, json);
            await _candidateService.CalculateAndSaveCompletionAsync(candidateId);
            TempData["SuccessMessage"] = "Cập nhật Học vấn thành công!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UpdateExperience(string Title, string Company, string StartDate, string EndDate, string Description)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int candidateId = int.Parse(userIdStr!);
            var json = System.Text.Json.JsonSerializer.Serialize(new[] { new { Title, Company, StartDate, EndDate, Description } });
            await _cvService.UpdateExperienceAsync(candidateId, json);
            await _candidateService.CalculateAndSaveCompletionAsync(candidateId);
            TempData["SuccessMessage"] = "Cập nhật Kinh nghiệm thành công!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UpdateSkills(string SkillName, string Proficiency)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int candidateId = int.Parse(userIdStr!);
            await _cvService.AddOrUpdateSkillAsync(candidateId, SkillName, Proficiency);
            await _candidateService.CalculateAndSaveCompletionAsync(candidateId);
            TempData["SuccessMessage"] = "Thêm Kỹ năng thành công!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UpdateLanguages(string Language, string Proficiency)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int candidateId = int.Parse(userIdStr!);
            var json = System.Text.Json.JsonSerializer.Serialize(new[] { new { Language, Proficiency } });
            await _cvService.UpdateLanguagesAsync(candidateId, json);
            await _candidateService.CalculateAndSaveCompletionAsync(candidateId);
            TempData["SuccessMessage"] = "Cập nhật Ngoại ngữ thành công!";
            return RedirectToAction("Profile");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult AppliedJobs()
        {
            return View("~/Views/Candidate/CandidateApplication/AppliedJobs.cshtml");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult SavedJobs()
        {
            return View("~/Views/Candidate/Bookmark/Index.cshtml");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult Interviews()
        {
            return View("~/Views/Candidate/CandidateInterview/Index.cshtml");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult Notifications()
        {
            return View("~/Views/Candidate/CandidateNotification/Index.cshtml");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult NotificationDetails(int id)
        {
            ViewBag.NotificationId = id;
            return View("~/Views/Candidate/CandidateNotification/Details.cshtml");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult ChangePassword()
        {
            return View("~/Views/Candidate/CandidateProfile/ChangePassword.cshtml");
        }
    }
}


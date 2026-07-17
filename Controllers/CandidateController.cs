//03/06/2026 DatTX
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using DevHub.ViewModels.Candidate;
using DevHub.Data;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers
{
    public class CandidateController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;
        private readonly ICandidateService _candidateService;
        private readonly ICvService _cvService;
        private readonly ICandidateSkillService _skillService;
        private readonly ICommonTechnologyService _techService;
        private readonly IRecommendationService _recommendationService;
        private readonly ItrecruitmentDbContext _context;

        public CandidateController(
            IAuthService authService,
            IWebHostEnvironment env,
            ICandidateService candidateService,
            ICvService cvService,
            ICandidateSkillService skillService,
            ICommonTechnologyService techService,
            IRecommendationService recommendationService,
            ItrecruitmentDbContext context)
        {
            _authService = authService;
            _env = env;
            _candidateService = candidateService;
            _cvService = cvService;
            _skillService = skillService;
            _techService = techService;
            _recommendationService = recommendationService;
            _context = context;
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int candidateId)) return RedirectToAction("Login", "Auth");

            var interviewService = HttpContext.RequestServices.GetService(typeof(DevHub.Services.Interfaces.IInterviewService)) as DevHub.Services.Interfaces.IInterviewService;
            if (interviewService != null) await interviewService.SyncInterviewStatusesAsync();

            // Basic Counts
            var appliedJobsCount = await _context.Applications.CountAsync(a => a.CandidateId == candidateId);
            var savedJobsCount = await _context.Bookmarks.CountAsync(b => b.CandidateId == candidateId);
            var approvedJobsCount = await _context.Applications.CountAsync(a => a.CandidateId == candidateId && (a.Status == "APPROVED" || a.Status == "FINISHED" || a.Status == "HIRED" || a.Status == "FAILED"));
            var rejectedJobsCount = await _context.Applications.CountAsync(a => a.CandidateId == candidateId && a.Status == "REJECTED");
            var hiredJobsCount = await _context.Applications.CountAsync(a => a.CandidateId == candidateId && a.Status == "HIRED");
            var failedJobsCount = await _context.Applications.CountAsync(a => a.CandidateId == candidateId && a.Status == "FAILED");

            // Graph Data - 6 Months
            var sixMonthsAgo = DateTime.Now.AddMonths(-5);
            var startOfSixMonthsAgo = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);
            
            var apps6Months = await _context.Applications
                .Where(a => a.CandidateId == candidateId && a.AppliedAt >= startOfSixMonthsAgo && (a.Status == "APPROVED" || a.Status == "FINISHED" || a.Status == "HIRED" || a.Status == "FAILED" || a.Status == "REJECTED"))
                .ToListAsync();

            var graph6Months = new DevHub.Models.GraphDataDto();
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = DateTime.Now.AddMonths(-i);
                var monthName = monthDate.ToString("MM/yyyy");
                graph6Months.Labels.Add(monthName);
                
                var approvedCount = apps6Months.Count(a => a.AppliedAt?.Year == monthDate.Year && a.AppliedAt?.Month == monthDate.Month && (a.Status == "APPROVED" || a.Status == "FINISHED" || a.Status == "HIRED" || a.Status == "FAILED"));
                var rejectedCount = apps6Months.Count(a => a.AppliedAt?.Year == monthDate.Year && a.AppliedAt?.Month == monthDate.Month && a.Status == "REJECTED");
                var hiredCount = apps6Months.Count(a => a.AppliedAt?.Year == monthDate.Year && a.AppliedAt?.Month == monthDate.Month && a.Status == "HIRED");
                var failedCount = apps6Months.Count(a => a.AppliedAt?.Year == monthDate.Year && a.AppliedAt?.Month == monthDate.Month && a.Status == "FAILED");
                
                graph6Months.ApprovedData.Add(approvedCount);
                graph6Months.RejectedData.Add(rejectedCount);
                graph6Months.HiredData.Add(hiredCount);
                graph6Months.FailedData.Add(failedCount);
            }

            // Graph Data - 1 Month (Option B: Current Month)
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

            var apps1Month = await _context.Applications
                .Where(a => a.CandidateId == candidateId && a.AppliedAt >= startOfMonth && (a.Status == "APPROVED" || a.Status == "FINISHED" || a.Status == "HIRED" || a.Status == "FAILED" || a.Status == "REJECTED"))
                .ToListAsync();

            var graph1Month = new DevHub.Models.GraphDataDto();
            for (int i = 1; i <= daysInMonth; i++)
            {
                graph1Month.Labels.Add($"{i}/{now.Month}");
                var approvedCount = apps1Month.Count(a => a.AppliedAt?.Day == i && (a.Status == "APPROVED" || a.Status == "FINISHED" || a.Status == "HIRED" || a.Status == "FAILED"));
                var rejectedCount = apps1Month.Count(a => a.AppliedAt?.Day == i && a.Status == "REJECTED");
                var hiredCount = apps1Month.Count(a => a.AppliedAt?.Day == i && a.Status == "HIRED");
                var failedCount = apps1Month.Count(a => a.AppliedAt?.Day == i && a.Status == "FAILED");
                
                graph1Month.ApprovedData.Add(approvedCount);
                graph1Month.RejectedData.Add(rejectedCount);
                graph1Month.HiredData.Add(hiredCount);
                graph1Month.FailedData.Add(failedCount);
            }

            var model = new DevHub.Models.DashboardViewModel
            {
                AppliedJobsCount = appliedJobsCount,
                SavedJobsCount = savedJobsCount,
                InterviewsCount = 0,
                ApprovedJobsCount = approvedJobsCount,
                RejectedJobsCount = rejectedJobsCount,
                HiredJobsCount = hiredJobsCount,
                FailedJobsCount = failedJobsCount,
                GraphData6Months = graph6Months,
                GraphData1Month = graph1Month
            };
            return View("~/Views/Candidate/CandidateDashboard/Index.cshtml", model);
        }

        // View profile
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login", "Auth");

            var candidate = await _candidateService.GetCandidateByIdAsync(userId);
            if (candidate == null)
                return NotFound("Ứng viên không tồn tại");

            var cv = await _cvService.GetCvByCandidateIdAsync(userId);
            var skills = await _skillService.GetSkillsAsync(userId);
            var allTechs = (await _techService.GetAllTechsAsync())
                           .Where(t => t.IsActive == true).ToList();

            var viewModel = new CandidateProfileViewModel
            {
                CandidateInfo = candidate,
                Cv = cv,
                ProfileCompletion = candidate.ProfileCompletion ?? 0,
                CandidateSkills = skills,
                AllTechnologies = allTechs
            };
            return View("~/Views/Candidate/CandidateProfile/Index.cshtml", viewModel);
        }

        // Upload avatar
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar != null && avatar.Length > 0)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                    return Json(new { success = false, message = "Không xác định được người dùng" });

                try
                {
                    // Validate File Size (max 5MB)
                    const int maxFileSize = 5 * 1024 * 1024;
                    if (avatar.Length > maxFileSize)
                        return Json(new { success = false, message = "Kích thước ảnh tối đa là 5MB." });

                    // Validate File Type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                        return Json(new { success = false, message = "Định dạng ảnh không hợp lệ. Vui lòng chọn .jpg, .jpeg, .png hoặc .gif." });

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/avatars/{uniqueFileName}";
                    await _authService.SyncCandidateAvatarAsync(userId, relativePath);
                    await _candidateService.CalculateAndSaveCompletionAsync(userId);

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

            if (model.ExpectedSalaryMin.HasValue && model.ExpectedSalaryMax.HasValue
                && model.ExpectedSalaryMax.Value < model.ExpectedSalaryMin.Value)
            {
                TempData["ErrorMessage"] = "Mức lương tối đa phải lớn hơn hoặc bằng mức lương tối thiểu!";
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
                    model.CvSearchable,
                    model.PreferredWorkingModel
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

        // Upload CV từ modal ứng tuyển — trả JSON (không redirect)
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> UploadCvModal(IFormFile cvFile)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int candidateId))
                    return Json(new { success = false, message = "Không xác định được người dùng." });

                await _cvService.UploadCvFileAsync(candidateId, cvFile, _env.WebRootPath);
                await _candidateService.CalculateAndSaveCompletionAsync(candidateId);

                // Lấy lại CV vừa upsert để lấy CvId
                var cv = await _cvService.GetCvByCandidateIdAsync(candidateId);
                return Json(new { success = true, cvId = cv?.CvId, message = "Tải CV lên thành công!" });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tải CV: " + ex.Message });
            }
        }

        // Add skill
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> AddSkill(int techId, string level)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int candidateId = int.Parse(userIdStr!);
                await _skillService.AddSkillAsync(candidateId, techId, level);
                await _candidateService.CalculateAndSaveCompletionAsync(candidateId);
                TempData["SuccessMessage"] = "Thêm kỹ năng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
            }
            return RedirectToAction("Profile");
        }

        // Remove skill
        [HttpPost]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> RemoveSkill(int techId)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int candidateId = int.Parse(userIdStr!);
                await _skillService.RemoveSkillAsync(candidateId, techId);
                await _candidateService.CalculateAndSaveCompletionAsync(candidateId);
                TempData["SuccessMessage"] = "Đã xóa kỹ năng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
            }
            return RedirectToAction("Profile");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult AppliedJobs()
        {
            return View("~/Views/Candidate/CandidateApplication/AppliedJobs.cshtml");
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> RecommendedJobs([FromServices] IBookmarkService bookmarkService, int page = 1)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int candidateId))
                return RedirectToAction("Login", "Auth");

            var recommendedJobs = await _recommendationService.GetRecommendedJobsAsync(candidateId);
            
            int pageSize = 3;
            int totalItems = recommendedJobs.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Đảm bảo page nằm trong giới hạn
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedJobs = recommendedJobs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalJobs = totalItems;
            ViewBag.BookmarkedJobIds = await bookmarkService.GetBookmarkedJobIdsAsync(candidateId);

            return View("~/Views/Candidate/RecommendedJob/Index.cshtml", pagedJobs);
        }

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult SavedJobs()
        {
            return RedirectToAction("Index", "Bookmark", new { area = "" });
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

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> NotificationSettings()
        {
            var email  = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            if (dbUser == null) return NotFound();

            ViewBag.EmailNotificationsEnabled = dbUser.EmailNotificationsEnabled;
            ViewBag.NotifSettingsPostUrl = "/Candidate/NotificationSettings";
            return View("~/Views/Candidate/CandidateProfile/NotificationSettings.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CANDIDATE,Candidate")]
        public async Task<IActionResult> NotificationSettings(bool emailEnabled = false)
        {
            var email  = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            if (dbUser == null) return NotFound();

            dbUser.EmailNotificationsEnabled = emailEnabled;
            await _context.SaveChangesAsync();

            TempData["Success"] = emailEnabled
                ? "Đã bật thông báo qua email."
                : "Đã tắt thông báo qua email.";
            return RedirectToAction("NotificationSettings");
        }
    }
}

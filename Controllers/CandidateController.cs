using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace DevHub.Controllers
{
    public class CandidateController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;

        public CandidateController(IAuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
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

        [Authorize(Roles = "CANDIDATE,Candidate")]
        public IActionResult Profile()
        {
            return View("~/Views/Candidate/CandidateProfile/Index.cshtml");
        }

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


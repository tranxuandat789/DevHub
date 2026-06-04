using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DevHub.Services.Interfaces;
using System.Security.Claims;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/profile")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;

        public CandidateProfileController(IAuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Candidate/CandidateProfile/Index.cshtml");
        }

        [HttpPost("avatar")]
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

        [HttpGet("change-password")]
        public IActionResult ChangePassword()
        {
            return View("~/Views/Candidate/CandidateProfile/ChangePassword.cshtml");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DevHub.Services.Interfaces;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/Settings")]
    [Authorize(Roles = "BUSINESS")]
    public class SettingsController : Controller
    {
        private readonly IAuthService _authService;
            private readonly IRecruiterService _recruiterService;
            private readonly IWebHostEnvironment _env;
            private readonly ILogger<SettingsController> _logger;

            public SettingsController(IAuthService authService, IRecruiterService recruiterService, IWebHostEnvironment env, ILogger<SettingsController> logger)
            {
                _authService = authService;
                _recruiterService = recruiterService;
                _env = env;
                _logger = logger;
            }

        [HttpGet("")]
        public async Task<IActionResult> Index(string tab = "account")
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = tab.ToLower();

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
            {
                return NotFound();
            }

            return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
        }

        [HttpPost("company")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCompany(DevHub.ViewModels.Recruiter.RecruiterProfileViewModel model, IFormFile? logoFile)
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "company";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);

            // handle logo upload
            if (logoFile != null && logoFile.Length > 0)
            {
                var allowed = new[] { "image/png", "image/jpeg" };
                if (!allowed.Contains(logoFile.ContentType) || logoFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "Logo không hợp lệ. Chỉ PNG/JPG dưới 5MB.");
                    return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads", "companylogo");
                try
                {
                    Directory.CreateDirectory(uploads);
                    var fileName = $"logo_{dbUser.Recruiter.RecruiterId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(logoFile.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fs = System.IO.File.Create(filePath))
                    {
                        await logoFile.CopyToAsync(fs);
                    }
                    model.CompanyLogoUrl = $"/uploads/companylogo/{fileName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving company logo for recruiter {RecruiterId}", dbUser.Recruiter.RecruiterId);
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu file. Vui lòng thử lại hoặc liên hệ quản trị.");
                    return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
                }
            }

            try
            {
                await _recruiterService.UpdateProfileAsync(dbUser.Recruiter, model);
                TempData["Success"] = "Cập nhật thông tin công ty thành công.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("TaxCode", ex.Message);
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
            }

            return RedirectToAction("Index", new { tab = "company" });
        }

        [HttpPost("license")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLicense(IFormFile? file_upload)
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "license";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            if (file_upload == null || file_upload.Length == 0)
            {
                TempData["Error"] = "Vui lòng cung cấp Giấy phép kinh doanh hợp lệ để thực hiện xác thực";
                return RedirectToAction("Index", new { tab = "license" });
            }

            var allowedExt = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var ext = Path.GetExtension(file_upload.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext) || file_upload.Length > 10 * 1024 * 1024)
            {
                TempData["Error"] = "File không hợp lệ. Chỉ PDF/JPG/PNG/DOC dưới 10MB.";
                return RedirectToAction("Index", new { tab = "license" });
            }

            var uploads = Path.Combine(_env.WebRootPath, "uploads", "license");
            try
            {
                Directory.CreateDirectory(uploads);
                var fileName = $"license_{dbUser.Recruiter.RecruiterId}_{DateTime.UtcNow.Ticks}{ext}";
                var filePath = Path.Combine(uploads, fileName);
                using (var fs = System.IO.File.Create(filePath))
                {
                    await file_upload.CopyToAsync(fs);
                }

                var vm = new DevHub.ViewModels.Recruiter.RecruiterProfileViewModel
                {
                    FullName = dbUser.Recruiter.FullName,
                    Position = dbUser.Recruiter.Position,
                    Phone = dbUser.Recruiter.Phone,
                    CompanyName = dbUser.Recruiter.CompanyName,
                    CompanyAddress = dbUser.Recruiter.CompanyAddress,
                    CompanyLogoUrl = dbUser.Recruiter.CompanyLogoUrl,
                    CompanyDescription = dbUser.Recruiter.CompanyDescription,
                    Website = dbUser.Recruiter.Website,
                    Industry = dbUser.Recruiter.Industry,
                    TaxCode = dbUser.Recruiter.TaxCode,
                    BusinessLicenseUrl = $"/uploads/license/{fileName}",
                    AdditionalDocumentsUrl = dbUser.Recruiter.AdditionalDocumentsUrl
                };

                await _recruiterService.UpdateProfileAsync(dbUser.Recruiter, vm);
                await _recruiterService.SendVerificationRequestAsync(dbUser.Recruiter.RecruiterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving license file for recruiter {RecruiterId}", dbUser.Recruiter.RecruiterId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải file. Vui lòng thử lại hoặc liên hệ quản trị.";
                return RedirectToAction("Index", new { tab = "license" });
            }

            TempData["Success"] = "Tải giấy phép thành công. Yêu cầu xác thực đã được gửi.";
            return RedirectToAction("Index", new { tab = "license" });
        }
    }
}

//AnhPT-02/06/2026
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "RECRUITER")]
    public class SettingsController : Controller
    {
        //Service instances
        private readonly IAuthService _authService;
        private readonly IRecruiterService _recruiterService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SettingsController> _logger;

        //Constructor Injection
        public SettingsController(IAuthService authService, IRecruiterService recruiterService, IWebHostEnvironment env, ILogger<SettingsController> logger)
        {
            _authService = authService;
            _recruiterService = recruiterService;
            _env = env;
            _logger = logger;
        }

        //Default tab in Settings is "account".
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

            // Google-login accounts (no local password) -> hide the "current password" box.
            ViewBag.IsGoogleAccount = string.IsNullOrEmpty(dbUser.PasswordHash) || dbUser.PasswordHash == "GOOGLE_OAUTH";
            ViewBag.HasPendingRequest = await _recruiterService.HasPendingVerificationRequestAsync(dbUser.Recruiter.RecruiterId);

            return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
        }

        [HttpPost("account")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount(RecruiterProfileViewModel model)
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "account";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            //remove validation state for company fields (do not in the request form) so these fields don't block the save.
            ModelState.Remove(nameof(model.CompanyName));
            ModelState.Remove(nameof(model.TaxCode));
            ModelState.Remove(nameof(model.Website));

            if (!ModelState.IsValid)
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);

            //new viewmodel entity for update, preserve fields.
            var vm = new DevHub.ViewModels.Recruiter.RecruiterProfileViewModel
            {
                FullName = model.FullName ?? dbUser.Recruiter.FullName,
                Position = model.Position,
                Phone = model.Phone,
                // preserve other fields of recruiter entity which are not in the account form.
                CompanyName = dbUser.Recruiter.CompanyName,
                CompanyAddress = dbUser.Recruiter.CompanyAddress,
                CompanyLogoUrl = dbUser.Recruiter.CompanyLogoUrl,
                CompanyDescription = dbUser.Recruiter.CompanyDescription,
                Website = dbUser.Recruiter.Website,
                Industry = dbUser.Recruiter.Industry,
                TaxCode = dbUser.Recruiter.TaxCode,
                BusinessLicenseUrl = dbUser.Recruiter.BusinessLicenseUrl,
                AdditionalDocumentsUrl = dbUser.Recruiter.AdditionalDocumentsUrl
            };

            await _recruiterService.UpdateProfileAsync(dbUser.Recruiter, vm);
            TempData["Success"] = "Cập nhật thông tin cá nhân thành công.";
            return RedirectToAction("Index", new { tab = "account" });
        }

        [HttpPost("company")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCompany(RecruiterProfileViewModel model, IFormFile? logoFile)
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "company";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            // FullName/Phone/Position are sent as hidden inputs only and overwritten from the DB.
            // Drop their validation to make sure do not block the save process of compoany information
            ModelState.Remove(nameof(model.FullName));
            ModelState.Remove(nameof(model.Phone));
            ModelState.Remove(nameof(model.Position));

            if (!ModelState.IsValid)
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);

            // Preserve fields do not in company form
            model.FullName = dbUser.Recruiter.FullName;
            model.Position = dbUser.Recruiter.Position;
            model.Phone = dbUser.Recruiter.Phone;
            model.BusinessLicenseUrl = dbUser.Recruiter.BusinessLicenseUrl;
            model.AdditionalDocumentsUrl = dbUser.Recruiter.AdditionalDocumentsUrl;

            // Preserve current logo; remember the old one so it can be deleted if replaced.
            var oldLogoUrl = dbUser.Recruiter.CompanyLogoUrl;
            model.CompanyLogoUrl = oldLogoUrl;

            // handle logo upload — override logo if upload new
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

                // New logo saved & DB updated -> delete the old logo file from wwwroot (if replaced).
                if (!string.IsNullOrEmpty(oldLogoUrl) && oldLogoUrl != model.CompanyLogoUrl
                    && oldLogoUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, oldLogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    catch (Exception delEx)
                    {
                        // Deletion failure must not break the update flow.
                        _logger.LogWarning(delEx, "Could not delete old company logo {Url} for recruiter {RecruiterId}", oldLogoUrl, dbUser.Recruiter.RecruiterId);
                    }
                }

                TempData["Success"] = "Cập nhật thông tin công ty thành công.";
            }
            catch (InvalidOperationException ex)
            {
                //Catch invalid tax code format
                ModelState.AddModelError("TaxCode", ex.Message);
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
            }

            return RedirectToAction("Index", new { tab = "company" });
        }

        // Change the recruiter login password (validates complexity + current password).
        [HttpPost("password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(RecruiterChangePasswordViewModel model)
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "password";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            // Google-login accounts have no current password -> skip its validation and hide the box.
            bool isGoogleOnly = string.IsNullOrEmpty(dbUser.PasswordHash) || dbUser.PasswordHash == "GOOGLE_OAUTH";
            ViewBag.IsGoogleAccount = isGoogleOnly;
            if (isGoogleOnly)
                ModelState.Remove(nameof(model.CurrentPassword));

            if (!ModelState.IsValid)
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);

            try
            {
                await _recruiterService.ChangePasswordAsync(dbUser.Recruiter.RecruiterId, model);
                TempData["Success"] = isGoogleOnly ? "Đặt mật khẩu thành công." : "Đổi mật khẩu thành công.";
                return RedirectToAction("Index", new { tab = "password" });
            }
            catch (InvalidOperationException ex)
            {
                // e.g. wrong current password, or Google-only account.
                ModelState.AddModelError("CurrentPassword", ex.Message);
                return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
            }
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

            //Limit file format
            var allowedExt = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var ext = Path.GetExtension(file_upload.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext) || file_upload.Length > 10 * 1024 * 1024)
            {
                //file is too large or wrong format
                TempData["Error"] = "File không hợp lệ. Chỉ PDF/JPG/PNG/DOC dưới 10MB.";
                return RedirectToAction("Index", new { tab = "license" });
            }

            //get path to save license files
            var uploads = Path.Combine(_env.WebRootPath, "uploads", "license");
            // Remember the current license file then delete it after the new one is saved.
            var oldLicenseUrl = dbUser.Recruiter.BusinessLicenseUrl;
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

                // New license uploaded would delete the old license file from wwwroot .
                if (!string.IsNullOrEmpty(oldLicenseUrl) && oldLicenseUrl != vm.BusinessLicenseUrl)
                {
                    try
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, oldLicenseUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    catch (Exception delEx)
                    {
                        // Deletion failure must not break the upload flow.
                        _logger.LogWarning(delEx, "Could not delete old license file {Url} for recruiter {RecruiterId}", oldLicenseUrl, dbUser.Recruiter.RecruiterId);
                    }
                }

                TempData["Success"] = "Tải giấy phép thành công.";
            }
            catch (Exception ex)
            {
                // exception while uploading file and logger for tracing
                _logger.LogError(ex, "Error saving license file for recruiter {RecruiterId}", dbUser.Recruiter.RecruiterId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải file. Vui lòng thử lại hoặc liên hệ quản trị.";
                return RedirectToAction("Index", new { tab = "license" });
            }
            return RedirectToAction("Index", new { tab = "license" });
        }

        [HttpPost("additional-doc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAdditionalDoc(IFormFile? additional_file)
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "license";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            if (additional_file == null || additional_file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn tài liệu bổ sung để tải lên.";
                return RedirectToAction("Index", new { tab = "license" });
            }

            var allowedExt = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var ext = Path.GetExtension(additional_file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext) || additional_file.Length > 10 * 1024 * 1024)
            {
                TempData["Error"] = "File không hợp lệ. Chỉ PDF/JPG/PNG/DOC dưới 10MB.";
                return RedirectToAction("Index", new { tab = "license" });
            }

            var uploads = Path.Combine(_env.WebRootPath, "uploads", "additional");
            // Remember the current additional doc so it can be deleted after the new one is saved.
            var oldDocUrl = dbUser.Recruiter.AdditionalDocumentsUrl;
            try
            {
                Directory.CreateDirectory(uploads);
                var fileName = $"additional_{dbUser.Recruiter.RecruiterId}_{DateTime.UtcNow.Ticks}{ext}";
                var filePath = Path.Combine(uploads, fileName);
                using (var fs = System.IO.File.Create(filePath))
                {
                    await additional_file.CopyToAsync(fs);
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
                    BusinessLicenseUrl = dbUser.Recruiter.BusinessLicenseUrl,   
                    AdditionalDocumentsUrl = $"/uploads/additional/{fileName}"     
                };

                await _recruiterService.UpdateProfileAsync(dbUser.Recruiter, vm);

                // New doc saved & DB updated -> delete the old additional doc from wwwroot (if replaced).
                if (!string.IsNullOrEmpty(oldDocUrl) && oldDocUrl != vm.AdditionalDocumentsUrl
                    && oldDocUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, oldDocUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    catch (Exception delEx)
                    {
                        // Deletion failure must not break the upload flow.
                        _logger.LogWarning(delEx, "Could not delete old additional doc {Url} for recruiter {RecruiterId}", oldDocUrl, dbUser.Recruiter.RecruiterId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving additional doc for recruiter {RecruiterId}", dbUser.Recruiter.RecruiterId);
                TempData["Error"] = "Đã xảy ra lỗi khi tải file. Vui lòng thử lại hoặc liên hệ quản trị.";
                return RedirectToAction("Index", new { tab = "license" });
            }

            TempData["Success"] = "Tài liệu bổ sung đã được cập nhật thành công.";
            return RedirectToAction("Index", new { tab = "license" });
        }

        // User-triggered verification request from the License tab.
        [HttpPost("request-verification")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestVerification()
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = "license";

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
                return NotFound();

            try
            {
                // Service applies the completeness (> 96%).
                await _recruiterService.SendVerificationRequestAsync(dbUser.Recruiter.RecruiterId);
                TempData["Success"] = "Đã gửi yêu cầu xác minh. Vui lòng chờ kiểm duyệt viên xử lý.";
            }
            catch (InvalidOperationException ex)
            {
                // e.g. completeness is not satisfied or missing business license
                TempData["Error"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Không tìm thấy thông tin nhà tuyển dụng.";
            }

            return RedirectToAction("Index", new { tab = "license" });
        }
    }
}

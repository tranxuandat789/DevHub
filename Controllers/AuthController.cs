using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DevHub.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _auth;
    private readonly ItrecruitmentDbContext _context;
    private readonly DevHub.Helpers.EmailHelper _emailHelper;

    private const string KeyGoogleEmail   = "GoogleEmail";
    private const string KeyGoogleId      = "GoogleId";
    private const string KeyGoogleName    = "GoogleName";
    private const string KeyGoogleAvatar  = "GoogleAvatar";
    private const string KeyGoogleFrom    = "GoogleFrom";

    public AuthController(IAuthService auth, DevHub.Helpers.EmailHelper emailHelper, ItrecruitmentDbContext context)
    {
        _context = context;
        _auth = auth;
        _emailHelper = emailHelper;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToDashboard();
        return View(new LoginViewModel());
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        var email    = vm.Email?.Trim() ?? "";
        var password = vm.Password?.Trim() ?? "";
        bool hasError = false;

        if (string.IsNullOrEmpty(email))
        {
            TempData["EmailError"] = "Email là bắt buộc";
            hasError = true;
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            TempData["EmailError"] = "Email không đúng định dạng (vd: abc@gmail.com)";
            hasError = true;
        }

        if (string.IsNullOrEmpty(password))
        {
            TempData["PasswordError"] = "Mật khẩu là bắt buộc";
            hasError = true;
        }

        if (hasError)
        {
            TempData["PrefillEmail"] = email;
            return RedirectToAction(nameof(Login));
        }

        var user = await _auth.FindUserByEmailAsync(email);
        if (user is null)
        {
            TempData["GeneralError"] = "Email hoặc mật khẩu không chính xác";
            TempData["PrefillEmail"] = email;
            return RedirectToAction(nameof(Login));
        }

        if (user.IsActive != true)
        {
            TempData["GeneralError"] = "Tài khoản đã bị khóa. Vui lòng liên hệ hỗ trợ.";
            return RedirectToAction(nameof(Login));
        }

        if (user.UserType?.Trim().ToUpper() == "RECRUITER")
        {
            TempData["GeneralError"] = "Tài khoản nhà tuyển dụng vui lòng đăng nhập tại trang dành riêng.";
            return RedirectToAction(nameof(Login));
        }

        if (user.PasswordHash == "GOOGLE_OAUTH" || string.IsNullOrEmpty(user.PasswordHash))
        {
            TempData["GeneralError"] = "Tài khoản này chỉ đăng nhập được bằng Google.";
            return RedirectToAction(nameof(Login));
        }

        if (!_auth.VerifyPassword(password, user.PasswordHash!))
        {
            TempData["GeneralError"] = "Email hoặc mật khẩu không chính xác";
            TempData["PrefillEmail"] = email;
            return RedirectToAction(nameof(Login));
        }

        await SignInAsync(user, vm.RememberMe);
        await _auth.UpdateLastLoginAsync(user.UserId);
        TempData["SuccessMsg"] = "Đăng nhập thành công! Chào mừng bạn trở lại.";

        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return RedirectToDashboard(user.UserType);
    }

    [HttpGet]
    public IActionResult EmployerLogin()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToDashboard();
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployerLogin(LoginViewModel vm, string? returnUrl = null)
    {
        var email    = vm.Email?.Trim() ?? "";
        var password = vm.Password?.Trim() ?? "";
        bool hasError = false;

        if (string.IsNullOrEmpty(email))
        {
            TempData["EmailError"] = "Email là bắt buộc";
            hasError = true;
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            TempData["EmailError"] = "Email không đúng định dạng (vd: abc@gmail.com)";
            hasError = true;
        }

        if (string.IsNullOrEmpty(password))
        {
            TempData["PasswordError"] = "Mật khẩu là bắt buộc";
            hasError = true;
        }

        if (hasError)
        {
            TempData["PrefillEmail"] = email;
            return RedirectToAction(nameof(EmployerLogin));
        }

        var user = await _auth.FindUserByEmailAsync(email);
        if (user is null)
        {
            TempData["GeneralError"] = "Email hoặc mật khẩu không chính xác";
            TempData["PrefillEmail"] = email;
            return RedirectToAction(nameof(EmployerLogin));
        }

        if (user.IsActive != true)
        {
            TempData["GeneralError"] = "Tài khoản đã bị khóa. Vui lòng liên hệ hỗ trợ.";
            return RedirectToAction(nameof(EmployerLogin));
        }

        if (user.UserType?.Trim().ToUpper() == "CANDIDATE")
        {
            TempData["GeneralError"] = "Tài khoản ứng viên vui lòng đăng nhập tại trang dành riêng.";
            return RedirectToAction(nameof(EmployerLogin));
        }

        if (user.PasswordHash == "GOOGLE_OAUTH" || string.IsNullOrEmpty(user.PasswordHash))
        {
            TempData["GeneralError"] = "Tài khoản này chỉ đăng nhập được bằng Google.";
            return RedirectToAction(nameof(EmployerLogin));
        }

        if (!_auth.VerifyPassword(password, user.PasswordHash!))
        {
            TempData["GeneralError"] = "Email hoặc mật khẩu không chính xác";
            TempData["PrefillEmail"] = email;
            return RedirectToAction(nameof(EmployerLogin));
        }

        await SignInAsync(user, vm.RememberMe);
        await _auth.UpdateLastLoginAsync(user.UserId);
        TempData["SuccessMsg"] = "Đăng nhập thành công! Chào mừng bạn trở lại.";

        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return RedirectToDashboard(user.UserType);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> EmployerLogout()
    {
        await HttpContext.SignOutAsync("EmployerCookies");
        return Redirect("/Home/Employer");
    }

    [HttpGet]
    public async Task<IActionResult> AdminLogout()
    {
        await HttpContext.SignOutAsync("AdminCookies");
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    public IActionResult GoogleLogin(string? from = "candidate")
    {
        HttpContext.Session.SetString(KeyGoogleFrom, from ?? "candidate");
        var props = new AuthenticationProperties { RedirectUri = "/Auth/GoogleCallback" };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync("ExternalCookie");
        if (!result.Succeeded)
        {
            TempData["ErrorMsg"] = "Đăng nhập Google thất bại.";
            return RedirectToAction(nameof(Login));
        }

        var googleId = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var email    = result.Principal!.FindFirstValue(ClaimTypes.Email) ?? "";
        var name     = result.Principal!.FindFirstValue(ClaimTypes.Name) ?? "";
        var avatar   = result.Principal!.FindFirstValue("picture") ?? "";
        var from     = HttpContext.Session.GetString(KeyGoogleFrom) ?? "candidate";

        if (string.IsNullOrEmpty(avatar) && result.Properties?.GetTokenValue("access_token") is string token)
        {
            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = await http.GetStringAsync("https://www.googleapis.com/oauth2/v3/userinfo");
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("picture", out var pic))
                    avatar = pic.GetString() ?? "";
            }
            catch { }
        }

        await HttpContext.SignOutAsync("ExternalCookie");

        if (string.IsNullOrEmpty(email))
        {
            TempData["ErrorMsg"] = "Không thể lấy email từ Google.";
            return RedirectToAction(nameof(Login));
        }

        var user = await _auth.FindUserByGoogleIdAsync(googleId)
                ?? await _auth.FindUserByEmailAsync(email);

        if (user is not null)
        {
            if (user.GoogleId is null)
                await _auth.LinkGoogleIdAsync(user.UserId, googleId);

            if (!string.IsNullOrEmpty(avatar) && user.UserType?.Trim().ToUpper() == "CANDIDATE"
                && user.Candidate?.ImageUrl != avatar)
            {
                await _auth.SyncCandidateAvatarAsync(user.UserId, avatar);
                if (user.Candidate != null) user.Candidate.ImageUrl = avatar;
            }

            if (!string.IsNullOrEmpty(avatar) && user.UserType?.Trim().ToUpper() == "RECRUITER"
                && user.Recruiter?.CompanyLogoUrl != avatar)
            {
                await _auth.SyncRecruiterAvatarAsync(user.UserId, avatar);
                if (user.Recruiter != null) user.Recruiter.CompanyLogoUrl = avatar;
            }

            if (user.IsActive != true)
            {
                TempData["GeneralError"] = "Tài khoản đã bị khóa.";
                return from == "employer"
                    ? Redirect("/Auth/EmployerLogin")
                    : RedirectToAction(nameof(Login));
            }

            if (from == "employer" && user.UserType?.Trim().ToUpper() == "CANDIDATE")
            {
                TempData["GeneralError"] = "Tài khoản ứng viên không thể đăng nhập tại trang nhà tuyển dụng.";
                return Redirect("/Auth/EmployerLogin");
            }

            if (from == "candidate" && user.UserType?.Trim().ToUpper() == "RECRUITER")
            {
                TempData["GeneralError"] = "Tài khoản nhà tuyển dụng vui lòng đăng nhập tại trang dành riêng.";
                return RedirectToAction(nameof(Login));
            }

            await SignInAsync(user, false, avatar);
            await _auth.UpdateLastLoginAsync(user.UserId);
            TempData["SuccessMsg"] = "Đăng nhập thành công! Chào mừng bạn trở lại.";
            return RedirectToDashboard(user.UserType);
        }

        if (from == "candidate")
        {
            var newUser = await _auth.CreateCandidateGoogleAccountAsync(email, googleId, name, avatar);
            await SignInAsync(newUser, false, avatar);
            TempData["SuccessMsg"] = "Đăng nhập thành công! Chào mừng bạn trở lại.";
            return RedirectToDashboard("Candidate");
        }

        HttpContext.Session.SetString(KeyGoogleEmail,  email);
        HttpContext.Session.SetString(KeyGoogleId,     googleId);
        HttpContext.Session.SetString(KeyGoogleName,   name);
        HttpContext.Session.SetString(KeyGoogleAvatar, avatar);
        return RedirectToAction(nameof(GoogleEmployerInfo));
    }

    [HttpGet]
    public IActionResult GoogleEmployerInfo()
    {
        var email = HttpContext.Session.GetString(KeyGoogleEmail);
        if (string.IsNullOrEmpty(email)) return Redirect("/Auth/EmployerLogin");

        return View(new GoogleEmployerInfoViewModel
        {
            Email    = email,
            FullName = HttpContext.Session.GetString(KeyGoogleName) ?? ""
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GoogleEmployerInfo(GoogleEmployerInfoViewModel vm)
    {
        var email    = HttpContext.Session.GetString(KeyGoogleEmail);
        var googleId = HttpContext.Session.GetString(KeyGoogleId);
        var name     = HttpContext.Session.GetString(KeyGoogleName) ?? "";
        var avatar   = HttpContext.Session.GetString(KeyGoogleAvatar) ?? "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
        {
            TempData["ErrorMsg"] = "Phiên đăng nhập hết hạn.";
            return Redirect("/Auth/EmployerLogin");
        }

        vm.Email    = email;
        vm.FullName = name;

        if (!ModelState.IsValid) return View(vm);

        var user = await _auth.CreateRecruiterGoogleAccountAsync(email, googleId, name, avatar, vm.Phone);

        HttpContext.Session.Remove(KeyGoogleEmail);
        HttpContext.Session.Remove(KeyGoogleId);
        HttpContext.Session.Remove(KeyGoogleName);
        HttpContext.Session.Remove(KeyGoogleAvatar);

        await SignInAsync(user, false, avatar);
        TempData["SuccessMsg"] = "Đăng ký tài khoản nhà tuyển dụng thành công!";
        return RedirectToDashboard("Recruiter");
    }

    private async Task SignInAsync(UserAccount user, bool rememberMe, string? googleAvatar = null)
    {
        var fullName = "";
        if (user.UserType?.Trim().ToUpper() == "CANDIDATE")
        {
            fullName = !string.IsNullOrEmpty(user.Candidate?.FullName) ? user.Candidate.FullName : user.Email;
        }
        else if (user.UserType?.Trim().ToUpper() == "RECRUITER")
        {
            fullName = !string.IsNullOrEmpty(user.Recruiter?.FullName) ? user.Recruiter.FullName : user.Email;
        }
        if (string.IsNullOrEmpty(fullName)) fullName = user.Email;

        var avatarUrl = "";
        if (user.UserType?.Trim().ToUpper() == "CANDIDATE")
        {
            avatarUrl = !string.IsNullOrEmpty(user.Candidate?.ImageUrl) ? user.Candidate.ImageUrl : googleAvatar;
        }
        else if (user.UserType?.Trim().ToUpper() == "RECRUITER")
        {
            avatarUrl = !string.IsNullOrEmpty(user.Recruiter?.CompanyLogoUrl) ? user.Recruiter.CompanyLogoUrl : googleAvatar;
        }
        avatarUrl ??= "";

        var (roleClaim, scheme) = (user.UserType?.Trim().ToUpper()) switch
        {
            "RECRUITER" => ("RECRUITER",  "EmployerCookies"),
            "ADMIN" => ("ADMIN",     "AdminCookies"),
            "MODERATOR" => ("MODERATOR", "AdminCookies"),
            _           => ("CANDIDATE", CookieAuthenticationDefaults.AuthenticationScheme)
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email,          user.Email),
            new(ClaimTypes.Role,           roleClaim),
            new(ClaimTypes.Name,           user.Email),
            new("FullName",                fullName),
            new("AvatarUrl",               avatarUrl)
        };

        if (roleClaim == "ADMIN")
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else if (roleClaim == "MODERATOR")
        {
            claims.Add(new Claim(ClaimTypes.Role, "Moderator"));
        }

        var identity  = new ClaimsIdentity(claims, scheme);
        var principal = new ClaimsPrincipal(identity);

        var props = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc   = DateTimeOffset.UtcNow.AddHours(rememberMe ? 720 : 12)
        };

        await HttpContext.SignInAsync(scheme, principal, props);
    }

    [HttpGet("Auth/ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("Auth/ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập email.";
            return View();
        }

        var user = await _auth.FindUserByEmailAsync(email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống.";
            return View();
        }

        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();
        
        // Store in session
        HttpContext.Session.SetString("ForgotPasswordEmail", email);
        HttpContext.Session.SetString("ForgotPasswordOTP", otp);
        HttpContext.Session.SetString("ForgotPasswordExpiry", DateTime.UtcNow.AddMinutes(5).ToString("O"));

        // Send email
        try
        {
            var subject = "Mã xác thực OTP đặt lại mật khẩu - DevHub";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: #4640DE; text-align: center;'>Đặt lại mật khẩu DevHub</h2>
                    <p>Xin chào,</p>
                    <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã OTP dưới đây để xác thực:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                    </div>
                    <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 5 phút.</p>
                    <p>Nếu bạn không gửi yêu cầu này, vui lòng bỏ qua email này.</p>
                    <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                </div>";
            
            await _emailHelper.SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to send email: {ex.Message}");
        }

        return RedirectToAction("VerifyOTP");
    }

    [HttpGet("Auth/VerifyOTP")]
    public IActionResult VerifyOTP()
    {
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("ForgotPassword");
        }
        ViewData["Email"] = email;
        return View();
    }

    [HttpPost("Auth/VerifyOTP")]
    public IActionResult VerifyOTP(string otp)
    {
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        var sessionOtp = HttpContext.Session.GetString("ForgotPasswordOTP");
        var expiryStr = HttpContext.Session.GetString("ForgotPasswordExpiry");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(expiryStr))
        {
            TempData["ErrorMessage"] = "Yêu cầu đã hết hạn. Vui lòng thực hiện lại.";
            return RedirectToAction("ForgotPassword");
        }

        ViewData["Email"] = email;

        if (!DateTime.TryParse(expiryStr, out var expiry) || DateTime.UtcNow > expiry)
        {
            TempData["ErrorMessage"] = "Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới.";
            return View();
        }

        if (sessionOtp != otp?.Trim())
        {
            TempData["ErrorMessage"] = "Mã OTP không chính xác.";
            return View();
        }

        // Mark as verified
        HttpContext.Session.SetString("OTPVerified", "true");
        return RedirectToAction("ResetPassword");
    }

    [HttpGet("Auth/ResetPassword")]
    public IActionResult ResetPassword()
    {
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        var verified = HttpContext.Session.GetString("OTPVerified");

        if (string.IsNullOrEmpty(email) || verified != "true")
        {
            return RedirectToAction("ForgotPassword");
        }

        return View();
    }

    [HttpPost("Auth/ResetPassword")]
    public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
    {
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        var verified = HttpContext.Session.GetString("OTPVerified");

        if (string.IsNullOrEmpty(email) || verified != "true")
        {
            return RedirectToAction("ForgotPassword");
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập mật khẩu mới.";
            return View();
        }

        if (newPassword.Contains(" "))
        {
            TempData["ErrorMessage"] = "Mật khẩu không được chứa khoảng trắng.";
            return View();
        }

        if (newPassword.Length < 8)
        {
            TempData["ErrorMessage"] = "Mật khẩu phải chứa ít nhất 8 ký tự.";
            return View();
        }

        if (!newPassword.Any(char.IsUpper))
        {
            TempData["ErrorMessage"] = "Mật khẩu phải chứa ít nhất 1 chữ cái viết hoa.";
            return View();
        }

        if (!newPassword.Any(c => !char.IsLetterOrDigit(c)))
        {
            TempData["ErrorMessage"] = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
            return View();
        }

        var user = await _auth.FindUserByEmailAsync(email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
            return View();
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _auth.UpdatePasswordAsync(user.UserId, hashedPassword);

        // Clear session
        HttpContext.Session.Remove("ForgotPasswordEmail");
        HttpContext.Session.Remove("ForgotPasswordOTP");
        HttpContext.Session.Remove("ForgotPasswordExpiry");
        HttpContext.Session.Remove("OTPVerified");

        TempData["SuccessMsg"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập bằng mật khẩu mới.";
        
        if (user.UserType?.Trim().ToUpper() == "RECRUITER")
        {
            return RedirectToAction("EmployerLogin");
        }
        return RedirectToAction("Login");
    }

    [HttpPost("Auth/ResendOTP")]
    public async Task<IActionResult> ResendOTP()
    {
        var email = HttpContext.Session.GetString("ForgotPasswordEmail");
        if (string.IsNullOrEmpty(email))
        {
            return Json(new { success = false, message = "Không tìm thấy email yêu cầu." });
        }

        var otp = new Random().Next(100000, 999999).ToString();
        HttpContext.Session.SetString("ForgotPasswordOTP", otp);
        HttpContext.Session.SetString("ForgotPasswordExpiry", DateTime.UtcNow.AddMinutes(5).ToString("O"));

        try
        {
            var subject = "Mã xác thực OTP đặt lại mật khẩu - DevHub";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: #4640DE; text-align: center;'>Đặt lại mật khẩu DevHub</h2>
                    <p>Xin chào,</p>
                    <p>Chúng tôi nhận được yêu cầu gửi lại mã OTP cho tài khoản của bạn. Vui lòng sử dụng mã OTP dưới đây để xác thực:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                    </div>
                    <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 5 phút.</p>
                    <p>Nếu bạn không gửi yêu cầu này, vui lòng bỏ quan email này.</p>
                    <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                </div>";

            await _emailHelper.SendEmailAsync(email, subject, body);
            return Json(new { success = true, message = "Mã OTP mới đã được gửi." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Gửi email thất bại: {ex.Message}" });
        }
    }

    private IActionResult RedirectToDashboard(string? userType = null)
    {
        userType ??= User.FindFirstValue(ClaimTypes.Role);

        return (userType?.Trim().ToUpper()) switch
        {
            "Admin"     or "ADMIN" => Redirect("/admin/dashboard"),
            "Moderator" or "MODERATOR" => Redirect("/moderator/job-approvals"),
            "Recruiter" or "RECRUITER" => Redirect("/Recruiter/Dashboard"),
            _                          => RedirectToAction("Index", "Home")
        };
    }
    [HttpGet]
    public IActionResult EmployerRegister()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> EmployerRegister(RegisterRecruiterViewModel registerRecruiter)
    {
        if (registerRecruiter == null)
        {
            ViewBag.ErrorMessage = "Dữ liệu đăng ký không hợp lệ!";
            return View();
        }

        string fullName = registerRecruiter.FullName ?? "";
        string phone = registerRecruiter.Phone ?? "";
        string email = registerRecruiter.Email ?? "";
        string passwordRaw = registerRecruiter.Password ?? "";
        string verifyPasswordRaw = registerRecruiter.VerifyPassword ?? "";
        string message = "";

        // Validate FullName (must have at least 2 words)
        var nameWords = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (nameWords.Length < 2)
        {
            message += "Họ và tên phải có ít nhất 2 từ! ";
        }

        // Validate Phone — format: 0xx xxxx xxx
        if (string.IsNullOrEmpty(phone) || !Regex.IsMatch(phone, @"^0\d{2} \d{4} \d{3}$"))
        {
            message += "Số điện thoại phải theo đúng định dạng '0xx xxxx xxx'! ";
        }

        // Validate Email
        if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|fpt\.edu\.vn)$"))
        {
            message += "Email phải theo định dạng example@gmail.com hoặc email doanh nghiệp example@fpt.edu.vn! ";
        }

        // Validate Password matching
        if (passwordRaw != verifyPasswordRaw)
        {
            message += "Mật khẩu và mật khẩu nhập lại không khớp! ";
        }

        // Validate Password strength
        if (string.IsNullOrEmpty(passwordRaw) || !Regex.IsMatch(passwordRaw, @"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{10,}$"))
        {
            message += "Mật khẩu phải dài ít nhất 10 ký tự, có ít nhất 1 chữ in hoa, 1 chữ số và 1 ký tự đặc biệt! ";
        }

        // Check if email already exists
        if (_context.UserAccounts.Any(u => u.Email == email))
        {
            message += "Email này đã được sử dụng để đăng ký tài khoản khác! ";
        }

        if (!string.IsNullOrEmpty(message))
        {
            ViewBag.ErrorMessage = message;
            return View(registerRecruiter);
        }

        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();
        Console.WriteLine($"[DevHub OTP] Generated OTP for {email}: {otp}");

        // Store registration info and OTP in session
        var registrationDataJson = JsonSerializer.Serialize(registerRecruiter);
        HttpContext.Session.SetString("EmployerRegisterData", registrationDataJson);
        HttpContext.Session.SetString("EmployerRegisterOTP", otp);
        HttpContext.Session.SetString("EmployerRegisterExpiry", DateTime.UtcNow.AddMinutes(15).ToString("O"));

        // Send OTP email
        try
        {
            var subject = "Mã xác thực OTP đăng ký Nhà tuyển dụng - DevHub";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: #4640DE; text-align: center;'>Xác thực đăng ký Nhà tuyển dụng DevHub</h2>
                    <p>Xin chào {fullName},</p>
                    <p>Cảm ơn bạn đã lựa chọn DevHub. Vui lòng sử dụng mã OTP dưới đây để hoàn tất quá trình đăng ký tài khoản nhà tuyển dụng của bạn:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                    </div>
                    <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                    <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>
                    <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                </div>";

            await _emailHelper.SendEmailAsync(email, subject, body);
            return RedirectToAction("EmployerVerifyOTP");
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            Console.WriteLine($"[DevHub EMAIL ERROR] Failed to send OTP email: {innerMsg}");
            ViewBag.ErrorMessage = $"Không thể gửi email xác thực đến {email}. Lỗi: {innerMsg}";
            return View(registerRecruiter);
        }
    }

    [HttpGet]
    public IActionResult EmployerVerifyOTP()
    {
        var dataJson = HttpContext.Session.GetString("EmployerRegisterData");
        if (string.IsNullOrEmpty(dataJson))
        {
            return RedirectToAction("EmployerRegister");
        }

        var registerRecruiter = JsonSerializer.Deserialize<RegisterRecruiterViewModel>(dataJson);
        ViewData["Email"] = registerRecruiter?.Email;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployerVerifyOTP(string otp)
    {
        var dataJson = HttpContext.Session.GetString("EmployerRegisterData");
        var sessionOtp = HttpContext.Session.GetString("EmployerRegisterOTP");
        var expiryStr = HttpContext.Session.GetString("EmployerRegisterExpiry");
        Console.WriteLine($"[DevHub Debug] Received OTP: '{otp}', Session OTP: '{sessionOtp}'");

        if (string.IsNullOrEmpty(dataJson) || string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(expiryStr))
        {
            TempData["ErrorMessage"] = "Yêu cầu đăng ký đã hết hạn hoặc không tìm thấy thông tin. Vui lòng đăng ký lại.";
            return RedirectToAction("EmployerRegister");
        }

        var registerRecruiter = JsonSerializer.Deserialize<RegisterRecruiterViewModel>(dataJson)!;
        ViewData["Email"] = registerRecruiter.Email;

        if (!DateTime.TryParse(expiryStr, out var expiry) || DateTime.UtcNow > expiry)
        {
            TempData["ErrorMessage"] = "Mã OTP đã hết hạn. Vui lòng yêu cầu gửi lại mã.";
            return View();
        }

        if (sessionOtp != otp?.Trim())
        {
            TempData["ErrorMessage"] = "Mã OTP không chính xác.";
            return View();
        }

        // Save to database
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Double check if email already exists (DB Conflict protection)
                if (await _context.UserAccounts.AnyAsync(u => u.Email == registerRecruiter.Email))
                {
                    TempData["ErrorMessage"] = "Email này đã được sử dụng để đăng ký tài khoản khác!";
                    return View();
                }

                var user = new UserAccount
                {
                    Email = registerRecruiter.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRecruiter.Password),
                    UserType = "RECRUITER",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    LastUpdated = DateTime.Now
                };
                _context.UserAccounts.Add(user);
                await _context.SaveChangesAsync(); // SaveChanges lần 1 để lấy UserId

                var recruiter = new DevHub.Models.Recruiter
                {
                    RecruiterId = user.UserId,
                    FullName = registerRecruiter.FullName,
                    Phone = registerRecruiter.Phone,
                    CompanyName = "Chưa cập nhật",
                    IsVerified = false,
                    ProfileCompletion = 0,
                    TotalSpent = 0,
                    AverageRating = 0,
                    TotalReviews = 0
                };
                _context.Recruiters.Add(recruiter);
                await _context.SaveChangesAsync(); // SaveChanges lần 2 để lưu Recruiter

                await transaction.CommitAsync();

                // Clear session keys
                HttpContext.Session.Remove("EmployerRegisterData");
                HttpContext.Session.Remove("EmployerRegisterOTP");
                HttpContext.Session.Remove("EmployerRegisterExpiry");

                // Sign in the user automatically
                await SignInAsync(user, false);
                TempData["SuccessMsg"] = "Đăng ký tài khoản nhà tuyển dụng thành công!";
                return RedirectToDashboard("Recruiter");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                if (innerMsg.Contains("UQ__user_acc__AB6E6164EFF0C5ED") || innerMsg.Contains("unique") || innerMsg.Contains("duplicate"))
                {
                    TempData["ErrorMessage"] = "Email này đã được sử dụng để đăng ký tài khoản khác!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Lỗi hệ thống khi tạo tài khoản: " + innerMsg;
                }
                return View();
            }
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployerResendOTP()
    {
        var dataJson = HttpContext.Session.GetString("EmployerRegisterData");
        if (string.IsNullOrEmpty(dataJson))
        {
            return Json(new { success = false, message = "Không tìm thấy thông tin đăng ký." });
        }

        var registerRecruiter = JsonSerializer.Deserialize<RegisterRecruiterViewModel>(dataJson)!;
        var otp = new Random().Next(100000, 999999).ToString();
        Console.WriteLine($"[DevHub OTP] Resent OTP for {registerRecruiter.Email}: {otp}");
        HttpContext.Session.SetString("EmployerRegisterOTP", otp);
        HttpContext.Session.SetString("EmployerRegisterExpiry", DateTime.UtcNow.AddMinutes(15).ToString("O"));

        try
        {
            var subject = "Mã xác thực OTP đăng ký Nhà tuyển dụng - DevHub";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                    <h2 style='color: #4640DE; text-align: center;'>Xác thực đăng ký Nhà tuyển dụng DevHub</h2>
                    <p>Xin chào {registerRecruiter.FullName},</p>
                    <p>Chúng tôi đã gửi lại mã xác thực OTP đăng ký tài khoản nhà tuyển dụng của bạn. Vui lòng sử dụng mã dưới đây:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                    </div>
                    <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                    <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>
                    <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                    <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
                </div>";

            await _emailHelper.SendEmailAsync(registerRecruiter.Email, subject, body);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Gửi email thất bại: {ex.Message}" });
        }
    }
}



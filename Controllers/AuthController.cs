// =========================================================================
// Các thư viện, package phục vụ xử lý backend đăng nhập, phiên đăng nhập
// Author: PhongDH
// Date: 31/06/2026
// =========================================================================
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

    private const string KeyGoogleEmail = "GoogleEmail";
    private const string KeyGoogleId = "GoogleId";
    private const string KeyGoogleName = "GoogleName";
    private const string KeyGoogleAvatar = "GoogleAvatar";
    private const string KeyGoogleFrom = "GoogleFrom";

    public AuthController(IAuthService auth, DevHub.Helpers.EmailHelper emailHelper, ItrecruitmentDbContext context)
    {
        _context = context;
        _auth = auth;
        _emailHelper = emailHelper;
    }


    /// <summary>
    /// Hiển thị trang đăng nhập cho ứng viên.
    /// </summary>
    /// <returns>View trang đăng nhập hoặc chuyển hướng đến Dashboard nếu đã đăng nhập.</returns>
    [HttpGet("Auth/Login")]
    [HttpGet("Account/Login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToDashboard();
        return View("LoginCandidate", new LoginViewModel());
    }

    [HttpGet]
    public IActionResult Register() => View("RegisterCandidate");

    /// <summary>
    /// Xử lý dữ liệu đăng nhập của ứng viên.
    /// </summary>
    /// <param name="vm">Dữ liệu đăng nhập (Email, Mật khẩu).</param>
    /// <param name="returnUrl">Đường dẫn trả về sau khi đăng nhập thành công (tùy chọn).</param>
    /// <returns>Chuyển hướng đến Dashboard hoặc trang trả về nếu thành công, ngược lại hiển thị lỗi.</returns>
    [HttpPost("Auth/Login")]
    [HttpPost("Account/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        var email = vm.Email?.Trim() ?? "";
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

    /// <summary>
    /// Hiển thị trang đăng nhập dành riêng cho nhà tuyển dụng.
    /// </summary>
    /// <returns>View trang đăng nhập hoặc chuyển hướng đến Dashboard nếu đã đăng nhập.</returns>
    [HttpGet]
    public IActionResult EmployerLogin()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToDashboard();
        return View(new LoginViewModel());
    }

    /// <summary>
    /// Xử lý dữ liệu đăng nhập của nhà tuyển dụng.
    /// </summary>
    /// <param name="vm">Dữ liệu đăng nhập (Email, Mật khẩu).</param>
    /// <param name="returnUrl">Đường dẫn trả về sau khi đăng nhập thành công (tùy chọn).</param>
    /// <returns>Chuyển hướng đến Dashboard nhà tuyển dụng hoặc trang trả về nếu thành công, ngược lại hiển thị lỗi.</returns>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployerLogin(LoginViewModel vm, string? returnUrl = null)
    {
        var email = vm.Email?.Trim() ?? "";
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

    /// <summary>
    /// Xử lý đăng xuất tài khoản ứng viên (xóa cookie xác thực).
    /// </summary>
    /// <returns>Chuyển hướng về trang chủ.</returns>
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Xử lý đăng xuất tài khoản nhà tuyển dụng (xóa cookie "EmployerCookies").
    /// </summary>
    /// <returns>Chuyển hướng về trang chủ dành cho nhà tuyển dụng.</returns>
    [HttpGet]
    public async Task<IActionResult> EmployerLogout()
    {
        await HttpContext.SignOutAsync("EmployerCookies");
        return Redirect("/Home/Employer");
    }

    /// <summary>
    /// Xử lý đăng xuất tài khoản quản trị viên/điều hành viên (xóa cookie "AdminCookies").
    /// </summary>
    /// <returns>Chuyển hướng về trang đăng nhập.</returns>
    [HttpGet]
    public async Task<IActionResult> AdminLogout()
    {
        await HttpContext.SignOutAsync("AdminCookies");
        return RedirectToAction("Login", "Auth");
    }

    /// <summary>
    /// Bắt đầu luồng xác thực đăng nhập bằng Google OAuth.
    /// </summary>
    /// <param name="from">Nguồn đăng nhập (ứng viên "candidate" hoặc nhà tuyển dụng "employer").</param>
    /// <returns>Chuyển hướng tới trang xác thực của Google.</returns>
    [HttpGet]
    public IActionResult GoogleLogin(string? from = "candidate")
    {
        HttpContext.Session.SetString(KeyGoogleFrom, from ?? "candidate");
        var props = new AuthenticationProperties { RedirectUri = "/Auth/GoogleCallback" };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// [DEV ONLY] Mô phỏng đăng nhập Google để test ở môi trường local không có OAuth key.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> TestGoogle(string? from = "candidate")
    {
        // Dữ liệu giả mô phỏng Google trả về
        var fakeEmail = from == "employer" ? "test.employer@gmail.com" : "test.candidate@gmail.com";
        var fakeName = from == "employer" ? "Test Employer" : "Test Candidate";
        var fakeGoogleId = from == "employer" ? "fake-google-id-employer" : "fake-google-id-candidate";
        var fakeAvatar = "";

        // Tìm user hiện có
        var user = await _auth.FindUserByEmailAsync(fakeEmail);

        if (user is not null)
        {
            if (user.IsActive != true)
            {
                TempData["GeneralError"] = "Tài khoản đã bị khóa.";
                return from == "employer"
                    ? Redirect("/Auth/EmployerLogin")
                    : RedirectToAction(nameof(Login));
            }
            await SignInAsync(user, false, fakeAvatar);
            await _auth.UpdateLastLoginAsync(user.UserId);
            return RedirectToDashboard(user.UserType);
        }

        // Tạo tài khoản mới
        if (from == "employer")
        {
            HttpContext.Session.SetString(KeyGoogleEmail, fakeEmail);
            HttpContext.Session.SetString(KeyGoogleId, fakeGoogleId);
            HttpContext.Session.SetString(KeyGoogleName, fakeName);
            HttpContext.Session.SetString(KeyGoogleAvatar, fakeAvatar);
            return RedirectToAction(nameof(GoogleEmployerInfo));
        }
        else
        {
            var newUser = await _auth.CreateCandidateGoogleAccountAsync(fakeEmail, fakeGoogleId, fakeName, fakeAvatar);
            await SignInAsync(newUser, false, fakeAvatar);
            return RedirectToDashboard("Candidate");
        }
    }


    /// <summary>
    /// Xử lý dữ liệu trả về từ Google sau khi người dùng xác thực thành công.
    /// </summary>
    /// <returns>
    /// Đăng nhập và chuyển hướng tới Dashboard nếu người dùng đã tồn tại.
    /// Tạo tài khoản mới tự động cho ứng viên.
    /// Chuyển hướng yêu cầu thêm thông tin đối với nhà tuyển dụng mới.
    /// </returns>
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
        var email = result.Principal!.FindFirstValue(ClaimTypes.Email) ?? "";
        var name = result.Principal!.FindFirstValue(ClaimTypes.Name) ?? "";
        var avatar = result.Principal!.FindFirstValue("picture") ?? "";
        var from = HttpContext.Session.GetString(KeyGoogleFrom) ?? "candidate";

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
                && user.Recruiter?.Company?.CompanyLogoUrl != avatar)
            {
                await _auth.SyncRecruiterAvatarAsync(user.UserId, avatar);
                if (user.Recruiter?.Company != null) user.Recruiter.Company.CompanyLogoUrl = avatar;
            }

            if (user.IsActive != true)
            {
                TempData["GeneralError"] = "Tài khoản đã bị khóa.";
                return from is "employer" or "register-employer"
                    ? Redirect("/Auth/EmployerLogin")
                    : RedirectToAction(nameof(Login));
            }

            // Nếu đến từ form đăng ký → đăng nhập thẳng vào đúng dashboard, không báo lỗi
            if (from is "register-candidate" or "register-employer")
            {
                await SignInAsync(user, false, avatar);
                await _auth.UpdateLastLoginAsync(user.UserId);
                return RedirectToDashboard(user.UserType);
            }

            // Đến từ form đăng nhập → kiểm tra đúng loại tài khoản
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

        // Tài khoản chưa tồn tại → tạo mới
        if (from is "candidate" or "register-candidate")
        {
            var newUser = await _auth.CreateCandidateGoogleAccountAsync(email, googleId, name, avatar);
            await SignInAsync(newUser, false, avatar);
            TempData["SuccessMsg"] = "Đăng nhập thành công! Chào mừng bạn trở lại.";
            return RedirectToDashboard("Candidate");
        }

        HttpContext.Session.SetString(KeyGoogleEmail, email);
        HttpContext.Session.SetString(KeyGoogleId, googleId);
        HttpContext.Session.SetString(KeyGoogleName, name);
        HttpContext.Session.SetString(KeyGoogleAvatar, avatar);
        return RedirectToAction(nameof(GoogleEmployerInfo));

    }

    /// <summary>
    /// Hiển thị form nhập thông tin bổ sung cho nhà tuyển dụng đăng nhập lần đầu bằng Google.
    /// </summary>
    /// <returns>View điền thông tin bổ sung.</returns>
    [HttpGet]
    public IActionResult GoogleEmployerInfo()
    {
        var email = HttpContext.Session.GetString(KeyGoogleEmail);
        if (string.IsNullOrEmpty(email)) return Redirect("/Auth/EmployerLogin");

        return View(new GoogleEmployerInfoViewModel
        {
            Email = email,
            FullName = HttpContext.Session.GetString(KeyGoogleName) ?? ""
        });
    }

    /// <summary>
    /// Xử lý lưu thông tin bổ sung và tạo tài khoản nhà tuyển dụng qua Google.
    /// </summary>
    /// <param name="vm">Dữ liệu thông tin nhà tuyển dụng (Số điện thoại, v.v.).</param>
    /// <returns>Đăng nhập và chuyển hướng tới Dashboard nhà tuyển dụng nếu thành công.</returns>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GoogleEmployerInfo(GoogleEmployerInfoViewModel vm)
    {
        var email = HttpContext.Session.GetString(KeyGoogleEmail);
        var googleId = HttpContext.Session.GetString(KeyGoogleId);
        var name = HttpContext.Session.GetString(KeyGoogleName) ?? "";
        var avatar = HttpContext.Session.GetString(KeyGoogleAvatar) ?? "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
        {
            TempData["ErrorMsg"] = "Phiên đăng nhập hết hạn.";
            return Redirect("/Auth/EmployerLogin");
        }

        vm.Email = email;
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

    /// <summary>
    /// Hàm dùng chung để khởi tạo phiên đăng nhập (Cookie Authentication) cho tất cả các loại tài khoản.
    /// Thiết lập các Claims (thông tin định danh) như: UserId, Email, Role, FullName, Avatar, và tạo Cookie theo loại User.
    /// </summary>
    /// <param name="user">Đối tượng người dùng đã xác thực thành công.</param>
    /// <param name="rememberMe">Cờ xác định có lưu thông tin đăng nhập (kéo dài thời gian sống của Cookie hay không).</param>
    /// <param name="googleAvatar">Avatar lấy từ Google (nếu có) để dự phòng khi user chưa có avatar.</param>
    /// <returns>Một tác vụ (Task) thực hiện ghi Cookie xuống client.</returns>
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
            avatarUrl = !string.IsNullOrEmpty(user.Recruiter?.Company?.CompanyLogoUrl) ? user.Recruiter.Company.CompanyLogoUrl : googleAvatar;
        }
        avatarUrl ??= "";

        var (roleClaim, scheme) = (user.UserType?.Trim().ToUpper()) switch
        {
            "RECRUITER" => ("RECRUITER", "EmployerCookies"),
            "ADMIN" => ("ADMIN", "AdminCookies"),
            "MODERATOR" => ("MODERATOR", "AdminCookies"),
            _ => ("CANDIDATE", CookieAuthenticationDefaults.AuthenticationScheme)
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

        var identity = new ClaimsIdentity(claims, scheme);
        var principal = new ClaimsPrincipal(identity);

        var props = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(rememberMe ? 720 : 12)
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
            var content = $@"
                <p>Xin chào,</p>
                <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã OTP dưới đây để xác thực:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 5 phút.</p>
                <p>Nếu bạn không gửi yêu cầu này, vui lòng bỏ qua email này.</p>";
            var body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Đặt lại mật khẩu DevHub", content);

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
            var content = $@"
                <p>Xin chào,</p>
                <p>Chúng tôi nhận được yêu cầu gửi lại mã OTP cho tài khoản của bạn. Vui lòng sử dụng mã OTP dưới đây để xác thực:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 5 phút.</p>
                <p>Nếu bạn không gửi yêu cầu này, vui lòng bỏ quan email này.</p>";
            var body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Đặt lại mật khẩu DevHub", content);

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
            "Admin" or "ADMIN" => Redirect("/AdminDashboard"),
            "Moderator" or "MODERATOR" => Redirect("/moderator/job-approvals"),
            "Recruiter" or "RECRUITER" => Redirect("/Recruiter/Dashboard"),
            "CANDIDATE" or "Candidate" => Redirect("/Candidate/Dashboard"),
            _ => RedirectToAction("Index", "Home")
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
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            ViewBag.ErrorMessage = string.Join("<br/>", errors);
            return View(registerRecruiter);
        }

        // Check email trùng
        if (_context.UserAccounts.Any(u => u.Email == registerRecruiter.Email))
        {
            ViewBag.ErrorMessage = "Email này đã được sử dụng để đăng ký tài khoản khác!";
            return View(registerRecruiter);
        }

        // Check số điện thoại trùng
        if (_context.Recruiters.Any(r => r.Phone == registerRecruiter.Phone))
        {
            ViewBag.ErrorMessage = "Số điện thoại này đã được sử dụng bởi tài khoản khác!";
            return View(registerRecruiter);
        }

        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();
        Console.WriteLine($"[DevHub OTP] Generated OTP for {registerRecruiter.Email}: {otp}");

        // Store registration info and OTP in session
        var registrationDataJson = JsonSerializer.Serialize(registerRecruiter);
        HttpContext.Session.SetString("EmployerRegisterData", registrationDataJson);
        HttpContext.Session.SetString("EmployerRegisterOTP", otp);
        HttpContext.Session.SetString("EmployerRegisterExpiry", DateTime.UtcNow.AddMinutes(15).ToString("O"));

        // Send OTP email
        try
        {
            var subject = "Mã xác thực OTP đăng ký Nhà tuyển dụng - DevHub";
            var content = $@"
                <p>Xin chào {registerRecruiter.FullName},</p>
                <p>Cảm ơn bạn đã lựa chọn DevHub. Vui lòng sử dụng mã OTP dưới đây để hoàn tất quá trình đăng ký tài khoản nhà tuyển dụng của bạn:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>";
            var body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Xác thực đăng ký Nhà tuyển dụng DevHub", content);

            await _emailHelper.SendEmailAsync(registerRecruiter.Email, subject, body);
            return RedirectToAction("EmployerVerifyOTP");
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            Console.WriteLine($"[DevHub EMAIL ERROR] Failed to send OTP email: {innerMsg}");
            ViewBag.ErrorMessage = $"Không thể gửi email xác thực đến {registerRecruiter.Email}. Lỗi: {innerMsg}";
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
                    LastUpdated = DateTime.Now
                };
                _context.UserAccounts.Add(user);
                await _context.SaveChangesAsync(); // SaveChanges lần 1 để lấy UserId

                var company = new DevHub.Models.Company
                {
                    CompanyName = registerRecruiter.CompanyName,
                    CompanyAddress = registerRecruiter.CompanyAddress,
                    Website = registerRecruiter.CompanyWebsite,
                    IsVerified = false,
                    ProfileCompletion = 0,
                    TotalSpent = 0,
                    AverageRating = 0,
                    TotalReviews = 0,
                    
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                var recruiter = new DevHub.Models.Recruiter
                {
                    RecruiterId = user.UserId,
                    FullName = registerRecruiter.FullName,
                    Phone = registerRecruiter.Phone,
                    CompanyId = company.CompanyId,
                    IsCompanyAdmin = true
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
            var content = $@"
                <p>Xin chào {registerRecruiter.FullName},</p>
                <p>Chúng tôi đã gửi lại mã xác thực OTP đăng ký tài khoản nhà tuyển dụng của bạn. Vui lòng sử dụng mã dưới đây:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>";
            var body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Xác thực đăng ký Nhà tuyển dụng DevHub", content);

            await _emailHelper.SendEmailAsync(registerRecruiter.Email, subject, body);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Gửi email thất bại: {ex.Message}" });
        }
    }


    public async Task<IActionResult> Register(RegisterCandidateViewModel vm)
    {
        // Kiểm tra ModelState (các annotation trên ViewModel đã lo validation cơ bản)
        if (!ModelState.IsValid)
            return View("RegisterCandidate", vm);

        var email = vm.Email.Trim().ToLower();

        // Kiểm tra email đã tồn tại chưa
        if (await _context.UserAccounts.AnyAsync(u => u.Email == email))
        {
            ViewBag.ErrorMessage = "Email này đã được sử dụng để đăng ký tài khoản khác!";
            return View("RegisterCandidate", vm);
            //return View(vm);
        }

        // Sinh OTP 6 chữ số
        var otp = new Random().Next(100000, 999999).ToString();
        Console.WriteLine($"[DevHub OTP] Candidate register OTP for {email}: {otp}");

        // Lưu thông tin đăng ký + OTP vào Session (giống Employer flow)
        var registrationDataJson = JsonSerializer.Serialize(vm);
        HttpContext.Session.SetString("CandidateRegisterData", registrationDataJson);
        HttpContext.Session.SetString("CandidateRegisterOTP", otp);
        HttpContext.Session.SetString("CandidateRegisterExpiry", DateTime.UtcNow.AddMinutes(15).ToString("O"));

        // Gửi email OTP
        try
        {
            var subject = "Mã xác thực OTP đăng ký Ứng viên - DevHub";
            var content = $@"
                <p>Xin chào {vm.FullName},</p>
                <p>Cảm ơn bạn đã lựa chọn DevHub. Vui lòng dùng mã OTP dưới đây để hoàn tất đăng ký:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>";
            var body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Xác thực đăng ký Ứng viên DevHub", content);

            await _emailHelper.SendEmailAsync(email, subject, body);
            return RedirectToAction(nameof(CandidateVerifyOTP));
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            Console.WriteLine($"[DevHub EMAIL ERROR] {innerMsg}");
            ViewBag.ErrorMessage = $"Không thể gửi email xác thực đến {email}. Lỗi: {innerMsg}";
            return View("RegisterCandidate", vm);
        }
    }

    /// <summary>
    /// Hiển thị trang nhập OTP cho ứng viên.
    /// </summary>
    [HttpGet]
    public IActionResult CandidateVerifyOTP()
    {
        var dataJson = HttpContext.Session.GetString("CandidateRegisterData");
        if (string.IsNullOrEmpty(dataJson))
            return RedirectToAction(nameof(Register));

        var vm = JsonSerializer.Deserialize<RegisterCandidateViewModel>(dataJson);
        ViewData["Email"] = vm?.Email;
        return View();
    }

    /// <summary>
    /// Xử lý xác thực OTP ứng viên.
    /// Kiểm tra OTP + hạn dùng → Tạo UserAccount + Candidate trong DB (transaction) → SignIn → Dashboard.
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CandidateVerifyOTP(string otp)
    {
        var dataJson = HttpContext.Session.GetString("CandidateRegisterData");
        var sessionOtp = HttpContext.Session.GetString("CandidateRegisterOTP");
        var expiryStr = HttpContext.Session.GetString("CandidateRegisterExpiry");
        Console.WriteLine($"[DevHub Debug] Candidate OTP received: '{otp}', session: '{sessionOtp}'");

        // Session hết hạn hoặc không còn dữ liệu
        if (string.IsNullOrEmpty(dataJson) || string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(expiryStr))
        {
            TempData["ErrorMessage"] = "Yêu cầu đăng ký đã hết hạn hoặc không tìm thấy thông tin. Vui lòng đăng ký lại.";
            return RedirectToAction(nameof(Register));
        }

        var vm = JsonSerializer.Deserialize<RegisterCandidateViewModel>(dataJson)!;
        ViewData["Email"] = vm.Email;

        // Kiểm tra OTP hết hạn
        if (!DateTime.TryParse(expiryStr, out var expiry) || DateTime.UtcNow > expiry)
        {
            TempData["ErrorMessage"] = "Mã OTP đã hết hạn. Vui lòng yêu cầu gửi lại mã.";
            return View();
        }

        // Kiểm tra OTP sai
        if (sessionOtp != otp?.Trim())
        {
            TempData["ErrorMessage"] = "Mã OTP không chính xác. Vui lòng thử lại.";
            return View();
        }

        // Lưu vào database với transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Bảo vệ race-condition: kiểm tra email lần cuối trước khi insert
            if (await _context.UserAccounts.AnyAsync(u => u.Email == vm.Email))
            {
                TempData["ErrorMessage"] = "Email này đã được sử dụng để đăng ký tài khoản khác!";
                await transaction.RollbackAsync();
                return View();
            }

            // Tạo UserAccount
            var user = new UserAccount
            {
                Email = vm.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password),
                UserType = "CANDIDATE",
                IsActive = true,
                LastUpdated = DateTime.Now
            };
            _context.UserAccounts.Add(user);
            await _context.SaveChangesAsync(); // lần 1 – lấy UserId

            // Tạo Candidate profile liên kết
            var candidate = new DevHub.Models.Candidate
            {
                CandidateId = user.UserId,
                FullName = vm.FullName.Trim(),
                Phone = vm.Phone.Trim(),
            };
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync(); // lần 2 – lưu Candidate

            await transaction.CommitAsync();

            // Xóa Session
            HttpContext.Session.Remove("CandidateRegisterData");
            HttpContext.Session.Remove("CandidateRegisterOTP");
            HttpContext.Session.Remove("CandidateRegisterExpiry");

            // Đăng nhập luôn sau khi đăng ký thành công
            await SignInAsync(user, false);
            TempData["SuccessMsg"] = $"Chào mừng {vm.FullName} đến với DevHub!";
            return RedirectToDashboard("CANDIDATE");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            var innerMsg = ex.InnerException?.Message ?? ex.Message;

            // Bắt lỗi unique constraint email từ DB
            if (innerMsg.Contains("unique", StringComparison.OrdinalIgnoreCase)
                || innerMsg.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
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

    /// <summary>
    /// Gửi lại OTP cho ứng viên (gọi bằng fetch từ client).
    /// Trả về JSON { success, message }.
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CandidateResendOTP()
    {
        var dataJson = HttpContext.Session.GetString("CandidateRegisterData");
        if (string.IsNullOrEmpty(dataJson))
            return Json(new { success = false, message = "Không tìm thấy thông tin đăng ký. Vui lòng đăng ký lại." });

        var vm = JsonSerializer.Deserialize<RegisterCandidateViewModel>(dataJson)!;
        var otp = new Random().Next(100000, 999999).ToString();
        Console.WriteLine($"[DevHub OTP] Resent candidate OTP for {vm.Email}: {otp}");

        HttpContext.Session.SetString("CandidateRegisterOTP", otp);
        HttpContext.Session.SetString("CandidateRegisterExpiry", DateTime.UtcNow.AddMinutes(15).ToString("O"));

        try
        {
            var subject = "Gửi lại mã xác thực OTP đăng ký Ứng viên - DevHub";
            var content = $@"
                <p>Xin chào {vm.FullName},</p>
                <p>Chúng tôi đã gửi lại mã xác thực OTP theo yêu cầu của bạn:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>";
            var body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Xác thực đăng ký Ứng viên DevHub", content);

            await _emailHelper.SendEmailAsync(vm.Email, subject, body);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Gửi email thất bại: {ex.Message}" });
        }
    }
}





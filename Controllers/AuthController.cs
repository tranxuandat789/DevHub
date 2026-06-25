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



    [HttpGet]
    public IActionResult Register() => View("RegisterCandidate");









































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
            var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #D6DDEB; border-radius: 8px;'>
                <h2 style='color: #4640DE; text-align: center;'>Xác thực đăng ký Nhà tuyển dụng DevHub</h2>
                <p>Xin chào {registerRecruiter.FullName},</p>
                <p>Cảm ơn bạn đã lựa chọn DevHub. Vui lòng sử dụng mã OTP dưới đây để hoàn tất quá trình đăng ký tài khoản nhà tuyển dụng của bạn:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE; background: #F7F5FC; padding: 15px 30px; border-radius: 8px; border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>
                <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
            </div>";

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


public async Task<IActionResult> Register(RegisterCandidateViewModel vm)
    {
        // Kiểm tra ModelState (các annotation trên ViewModel đã lo validation cơ bản)
        if (!ModelState.IsValid)
            return View("RegisterCandidate",vm);

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
            var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;
                        border: 1px solid #D6DDEB; border-radius: 8px;'>
                <h2 style='color: #4640DE; text-align: center;'>Xác thực đăng ký Ứng viên DevHub</h2>
                <p>Xin chào {vm.FullName},</p>
                <p>Cảm ơn bạn đã lựa chọn DevHub. Vui lòng dùng mã OTP dưới đây để hoàn tất đăng ký:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE;
                                 background: #F7F5FC; padding: 15px 30px; border-radius: 8px;
                                 border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>
                <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
            </div>";

            await _emailHelper.SendEmailAsync(email, subject, body);
            return RedirectToAction(nameof(CandidateVerifyOTP));
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            Console.WriteLine($"[DevHub EMAIL ERROR] {innerMsg}");
            ViewBag.ErrorMessage = $"Không thể gửi email xác thực đến {email}. Lỗi: {innerMsg}";
            return View("RegisterCandidate",vm);
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
                CreatedAt = DateTime.Now,
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
            var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;
                        border: 1px solid #D6DDEB; border-radius: 8px;'>
                <h2 style='color: #4640DE; text-align: center;'>Xác thực đăng ký Ứng viên DevHub</h2>
                <p>Xin chào {vm.FullName},</p>
                <p>Chúng tôi đã gửi lại mã xác thực OTP theo yêu cầu của bạn:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4640DE;
                                 background: #F7F5FC; padding: 15px 30px; border-radius: 8px;
                                 border: 1px dashed #4640DE;'>{otp}</span>
                </div>
                <p style='color: #FF3B30;'>Mã OTP này có hiệu lực trong vòng 15 phút.</p>
                <p>Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email này.</p>
                <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
            </div>";

            await _emailHelper.SendEmailAsync(vm.Email, subject, body);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Gửi email thất bại: {ex.Message}" });
        }
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
            ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
        };

        await HttpContext.SignInAsync(scheme, principal, props);
    }

    private IActionResult RedirectToDashboard(string? userType = null)
    {
        userType ??= User.FindFirstValue(ClaimTypes.Role);

        return (userType?.Trim().ToUpper()) switch
        {
            "Admin" or "ADMIN" => Redirect("/AdminDashboard"),
            "Moderator" or "MODERATOR" => Redirect("/moderator/job-approvals"),
            "Recruiter" or "RECRUITER" => Redirect("/Recruiter/Dashboard"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}


// =========================================================================
// Các thư viện, package phục vụ xử lý backend đăng nhập, phiên đăng nhập
// Author: PhongDH
// Date: 31/06/2026
// =========================================================================
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;
using DevHub.Repositories.Interfaces;
using DevHub.Repositories.Implementations;
using DevHub.Services.Interfaces;
using DevHub.Services.Implementations;
using System.Security.Claims;
using DevHub.Services.BackgroundServices;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Add SignalR

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

// Register DbContext
builder.Services.AddDbContext<ItrecruitmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Authentication — SmartAuth + 3 Cookie Schemes + Google ──────────────────
// [SmartAuth - Cơ chế điều hướng xác thực thông minh]
// Vấn đề: Ứng dụng này có 3 nhóm người dùng biệt lập (Ứng viên, Nhà tuyển dụng, Admin/Moderator). 
// Nếu dùng chung 1 cookie mặc định, các phiên đăng nhập sẽ đụng độ nhau (ví dụ: login ứng viên nhưng có thể vào trang admin nếu không chặn kĩ).
// Giải pháp: Sử dụng "Policy Scheme" (được đặt tên là "SmartAuth").
// SmartAuth không trực tiếp lưu Cookie, nó chỉ đóng vai trò "Trạm kiểm soát trung chuyển".
// Dựa vào đường dẫn (URL) mà người dùng đang truy cập, nó sẽ tự động trỏ đến đúng kho Cookie của nhóm người dùng đó.
builder.Services.AddAuthentication(options =>
{
    // Đặt SmartAuth làm Scheme mặc định để nó đón toàn bộ request cần xác thực
    options.DefaultScheme          = "SmartAuth";
    options.DefaultChallengeScheme = "SmartAuth";
})
.AddPolicyScheme("SmartAuth", "Smart Auth", options =>
{
    // ForwardDefaultSelector là nơi quyết định loại Cookie nào sẽ được áp dụng cho request hiện tại
    options.ForwardDefaultSelector = ctx =>
    {
        var path = ctx.Request.Path.Value ?? "";
        
        // Luồng 1: Nếu URL thuộc khu vực Nhà tuyển dụng (bắt đầu bằng /Recruiter hoặc chứa từ 'employer')
        // => Trỏ về "EmployerCookies". Nếu chưa đăng nhập, user sẽ bị đẩy về trang /Auth/EmployerLogin
        if (path.StartsWith("/Recruiter", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("employer", StringComparison.OrdinalIgnoreCase))
            return "EmployerCookies";
            
        // Luồng 2: Nếu URL thuộc khu vực Quản trị (bắt đầu bằng /Admin, /Moderator...)
        // => Trỏ về "AdminCookies".
        if (path.StartsWith("/Admin",     StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Moderator", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("moderator", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/Auth/AdminLogout", StringComparison.OrdinalIgnoreCase))
            return "AdminCookies";
            
        // Luồng 3: Nếu không phải các trường hợp đặc biệt trên (trang chủ, trang tìm việc...)
        // => Sử dụng Cookie mặc định của hệ thống (dành cho Ứng viên).
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
{
    o.Cookie.Name       = "App.Candidate";
    o.LoginPath         = "/Auth/Login";
    o.AccessDeniedPath  = "/";
    o.ExpireTimeSpan    = TimeSpan.FromHours(12);
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = ctx =>
    {
        var uri = ctx.RedirectUri;
        if (!uri.Contains("authRequired"))
            uri += (uri.Contains('?') ? "&" : "?") + "authRequired=1";
        ctx.Response.Redirect(uri);
        return Task.CompletedTask;
    };
})
.AddCookie("EmployerCookies", o =>
{
    o.Cookie.Name       = "App.Employer";
    o.LoginPath         = "/Auth/EmployerLogin";
    o.AccessDeniedPath  = "/Auth/EmployerLogin";
    o.ExpireTimeSpan    = TimeSpan.FromHours(12);
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = ctx =>
    {
        var uri = ctx.RedirectUri;
        if (!uri.Contains("authRequired"))
            uri += (uri.Contains('?') ? "&" : "?") + "authRequired=1";
        ctx.Response.Redirect(uri);
        return Task.CompletedTask;
    };
})
.AddCookie("AdminCookies", o =>
{
    o.Cookie.Name       = "App.Admin";
    o.LoginPath         = "/Auth/Login";
    o.ExpireTimeSpan    = TimeSpan.FromHours(12);
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = 404; return Task.CompletedTask;
    };
    o.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = 404; return Task.CompletedTask;
    };
})
.AddCookie("ExternalCookie", o =>
{
    o.ExpireTimeSpan      = TimeSpan.FromMinutes(5);
    o.Cookie.SameSite     = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    o.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
});

var googleClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
if (!string.IsNullOrEmpty(googleClientId))
{
    builder.Services.AddAuthentication()
        .AddGoogle(GoogleDefaults.AuthenticationScheme, o =>
        {
            o.SignInScheme  = "ExternalCookie";
            o.ClientId      = googleClientId;
            o.ClientSecret  = builder.Configuration["Authentication:Google:ClientSecret"]!;
            o.CallbackPath  = "/signin-google";
            o.Scope.Add("email");
            o.Scope.Add("profile");
            o.SaveTokens    = true;
            o.CorrelationCookie.SameSite     = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            o.CorrelationCookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            o.Events.OnCreatingTicket = ctx =>
            {
                if (ctx.User.TryGetProperty("picture", out var pic))
                {
                    var url = pic.GetString();
                    if (!string.IsNullOrEmpty(url))
                        ctx.Identity?.AddClaim(new Claim("picture", url));
                }
                return Task.CompletedTask;
            };
            o.Events.OnRemoteFailure = ctx =>
            {
                ctx.HandleResponse();
                var from = ctx.Request.Query["state"].ToString().Contains("employer") ? "employer" : "candidate";
                var redirectUrl = from == "employer" ? "/Auth/EmployerLogin" : "/Auth/Login";
                ctx.Response.Redirect(redirectUrl);
                return Task.CompletedTask;
            };
        });
}


builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("/Views/Candidate/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Recruiter/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Moderator/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
});

// Register Repositories
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICandidateSkillRepository, CandidateSkillRepository>();
builder.Services.AddScoped<ICommonJobPositionRepository, CommonJobPositionRepository>();
builder.Services.AddScoped<ICommonTechnologyRepository, CommonTechnologyRepository>();
builder.Services.AddScoped<ICvRepository, CvRepository>();
builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IJobPostRepository, JobPostRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPackageTransactionRepository, PackageTransactionRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IRecruiterRepository, RecruiterRepository>();
builder.Services.AddScoped<ICompanyPackageHistoryRepository, CompanyPackageHistoryRepository>();
builder.Services.AddScoped<IRecruiterDashboardRepository, RecruiterDashboardRepository>();
builder.Services.AddScoped<IReviewCompanyRepository, ReviewCompanyRepository>();
builder.Services.AddScoped<IServicePackageRepository, ServicePackageRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IRecruiterJobPostRepository, RecruiterJobPostRepository>();
builder.Services.AddScoped<IJobSearchRepository, JobSearchRepository>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<IRecruiterApplicationRepository, RecruiterApplicationRepository>();
builder.Services.AddScoped<ICompanyInvitationRepository, CompanyInvitationRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register Services
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ICandidateSkillService, CandidateSkillService>();
builder.Services.AddScoped<ICommonJobPositionService, CommonJobPositionService>();
builder.Services.AddScoped<ICommonTechnologyService, CommonTechnologyService>();
builder.Services.AddScoped<ICvService, CvService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IJobPostService, JobPostService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPackageTransactionService, PackageTransactionService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IRecruiterService, RecruiterService>();
builder.Services.AddScoped<ICompanyPackageHistoryService, CompanyPackageHistoryService>();
builder.Services.AddScoped<IReviewCompanieservice, ReviewCompanieservice>();
builder.Services.AddScoped<IServicePackageService, ServicePackageService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<DevHub.Helpers.EmailHelper>();
builder.Services.AddScoped<IRecruiterJobPostService, RecruiterJobPostService>();
builder.Services.AddScoped<IJobSearchService, JobSearchService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IRecruiterApplicationService, RecruiterApplicationService>();
builder.Services.AddScoped<IRecruiterDashboardService, RecruiterDashboardService>();
builder.Services.AddScoped<ICompanyInvitationService, CompanyInvitationService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IModAssignmentService, ModAssignmentService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


// Background worker: auto-close APPROVED job posts whose deadline has passed.
builder.Services.AddHostedService<JobPostAutoCloseService>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error404");
app.UseHttpsRedirection();

// Legacy URL compatibility: rewrite /uploads/company-logos/... to /uploads/companylogo/...
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (path.StartsWith("/uploads/company-logos/", StringComparison.OrdinalIgnoreCase))
    {
        var newPath = "/uploads/companylogo/" + path.Substring("/uploads/company-logos/".Length);
        context.Request.Path = new PathString(newPath);
    }
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseSession();           // ← phải trước Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<DevHub.Hubs.NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

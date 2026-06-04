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

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme          = "SmartAuth";
    options.DefaultChallengeScheme = "SmartAuth";
})
.AddPolicyScheme("SmartAuth", "Smart Auth", options =>
{
    options.ForwardDefaultSelector = ctx =>
    {
        var path = ctx.Request.Path.Value ?? "";
        if (path.StartsWith("/Recruiter", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("employer", StringComparison.OrdinalIgnoreCase))
            return "EmployerCookies";
        if (path.StartsWith("/Admin",     StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Moderator", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("moderator", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/Auth/AdminLogout", StringComparison.OrdinalIgnoreCase))
            return "AdminCookies";
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
builder.Services.AddScoped<IRecruiterPackageHistoryRepository, RecruiterPackageHistoryRepository>();
builder.Services.AddScoped<IReviewRecruiterRepository, ReviewRecruiterRepository>();
builder.Services.AddScoped<IServicePackageRepository, ServicePackageRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IRecruiterJobPostRepository, RecruiterJobPostRepository>();
builder.Services.AddScoped<IJobSearchRepository, JobSearchRepository>();

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
builder.Services.AddScoped<IRecruiterPackageHistoryService, RecruiterPackageHistoryService>();
builder.Services.AddScoped<IReviewRecruiterService, ReviewRecruiterService>();
builder.Services.AddScoped<IServicePackageService, ServicePackageService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<DevHub.Helpers.EmailHelper>();
builder.Services.AddScoped<IRecruiterJobPostService, RecruiterJobPostService>();
builder.Services.AddScoped<IJobSearchService, JobSearchService>();


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

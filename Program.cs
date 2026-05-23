using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;
using DevHub.Repositories.Interfaces;
using DevHub.Repositories.Implementations;
using DevHub.Services.Interfaces;
using DevHub.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ItrecruitmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath         = "/auth/login";
        options.LogoutPath        = "/auth/logout";
        options.AccessDeniedPath  = "/auth/access-denied";
        options.ExpireTimeSpan    = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Configure View Locations for Role-based folders
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // Role-based locations — thêm trước fallback để ưu tiên tìm theo role
    options.ViewLocationFormats.Add("/Views/Candidate/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Recruiter/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Moderator/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
    // Fallback chuẩn: Auth, Home, Job, Company, Blog
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
builder.Services.AddScoped<ICoinTransactionRepository, CoinTransactionRepository>();
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

// Register Services
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ICandidateSkillService, CandidateSkillService>();
builder.Services.AddScoped<ICoinTransactionService, CoinTransactionService>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

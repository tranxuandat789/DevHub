using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.Repositories.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/dashboard")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterDashboardController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRecruiterDashboardRepository _dashboardRepo;
        private readonly IRecruiterRepository _recruiterRepo;
        private readonly IRecruiterPackageHistoryRepository _packageRepo;

        public RecruiterDashboardController(
            IAuthService authService,
            IRecruiterDashboardRepository dashboardRepo,
            IRecruiterRepository recruiterRepo,
            IRecruiterPackageHistoryRepository packageRepo)
        {
            _authService = authService;
            _dashboardRepo = dashboardRepo;
            _recruiterRepo = recruiterRepo;
            _packageRepo = packageRepo;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Resolve the logged-in recruiter.
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser?.Recruiter == null)
                return NotFound();

            var recruiter = dbUser.Recruiter;
            int recruiterId = recruiter.RecruiterId;

            // 2. Pull real data.
            var posts = await _dashboardRepo.GetJobPostsAsync(recruiterId);        // all posts, newest first
            var interviews = await _dashboardRepo.GetInterviewsAsync(recruiterId);

            var scheduled = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "SCHEDULED" || s == "PENDING"; })
                .ToList();
            var completed = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "COMPLETED" || s == "CLOSED"; })
                .ToList();

            // Recent posts shown in the table (top 5 newest) + their applicant avatars.
            var recentPosts = posts.Take(5).ToList();
            var avatarsByJob = await _dashboardRepo.GetApplicantAvatarsByJobAsync(recentPosts.Select(p => p.JobId).ToList(), 3);

            var jobApplicantCounts = recentPosts.Select(j => new JobPostApplicantCount
            {
                JobId = j.JobId,
                Title = j.Title,
                ApplicantCount = j.ApplicationCount ?? 0,
                Status = j.Status,
                CreatedAt = j.CreatedAt,
                Deadline = j.Deadline,
                ApplicantAvatars = avatarsByJob.TryGetValue(j.JobId, out var avts) ? avts : new List<string>()
            }).ToList();

            // [#3] Active package, [#5] expiring jobs, [#6] recent applicants, [#10] pending verification.
            var package = await _packageRepo.GetActivePackageForRecruiterAsync(recruiterId);
            var expiringJobs = await _dashboardRepo.GetExpiringJobsAsync(recruiterId);
            var recentApps = await _dashboardRepo.GetRecentApplicationsAsync(recruiterId);
            bool hasPending = await _recruiterRepo.HasPendingVerificationRequestAsync(recruiterId);

            // [#4] Missing profile fields.
            var missingFields = new List<string>();
            if (string.IsNullOrEmpty(recruiter.CompanyLogoUrl)) missingFields.Add("Logo công ty");
            if (string.IsNullOrEmpty(recruiter.CompanyDescription)) missingFields.Add("Giới thiệu công ty");
            if (string.IsNullOrEmpty(recruiter.Website)) missingFields.Add("Website");
            if (string.IsNullOrEmpty(recruiter.Industry)) missingFields.Add("Ngành nghề");
            if (string.IsNullOrEmpty(recruiter.TaxCode)) missingFields.Add("Mã số thuế");
            if (string.IsNullOrEmpty(recruiter.BusinessLicenseUrl)) missingFields.Add("Giấy phép kinh doanh");

            // [#1] 30-day activity series (applications & interviews per day, zero-filled).
            const int statsDays = 30;
            var statsFrom = DateTime.Today.AddDays(-(statsDays - 1));
            var appDates = await _dashboardRepo.GetApplicationDatesAsync(recruiterId, statsFrom);
            var appByDay = appDates.GroupBy(d => d.Date).ToDictionary(g => g.Key, g => g.Count());
            var interviewByDay = interviews
                .Select(i => (i.CreatedAt ?? i.ScheduledTime).Date)
                .Where(d => d >= statsFrom.Date)
                .GroupBy(d => d)
                .ToDictionary(g => g.Key, g => g.Count());

            var statsLabels = new List<string>();
            var statsApplications = new List<int>();
            var statsInterviews = new List<int>();
            for (int i = 0; i < statsDays; i++)
            {
                var day = statsFrom.Date.AddDays(i);
                statsLabels.Add(day.ToString("d/M"));
                statsApplications.Add(appByDay.TryGetValue(day, out var ac) ? ac : 0);
                statsInterviews.Add(interviewByDay.TryGetValue(day, out var ic) ? ic : 0);
            }

            // 3. Assemble the dashboard model.
            var viewModel = new RecruiterDashboard
            {
                TotalJobPosts            = posts.Count,
                TotalApplications        = posts.Sum(j => j.ApplicationCount ?? 0),
                TotalScheduledInterviews = scheduled.Count,
                TotalCompletedInterviews = completed.Count,

                JobPostApplicantCounts = jobApplicantCounts,
                ScheduledInterviews = scheduled,
                CompletedInterviews = completed,

                IsVerified             = recruiter.IsVerified ?? false,
                HasPendingVerification = hasPending,

                HasActivePackage  = package != null,
                ActivePackageName = package?.Service?.PackageName,
                PostsRemaining    = package?.PostsRemaining ?? 0,
                PostsGranted      = package?.PostsGranted ?? 0,
                PackageExpiry     = package?.EndDate,

                ProfileCompletion    = recruiter.ProfileCompletion ?? 0,
                MissingProfileFields = missingFields,

                ExpiringJobs       = expiringJobs,
                RecentApplications = recentApps,

                StatsLabels        = statsLabels,
                StatsApplications  = statsApplications,
                StatsInterviews    = statsInterviews
            };

            return View("~/Views/Recruiter/RecruiterDashboard/Index.cshtml", viewModel);
        }
    }
}

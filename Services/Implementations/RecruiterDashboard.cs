//AnhPT-03/06/2026
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations
{
    // Business logic for the recruiter dashboard. All repository access + chart-data building
    // lives here so the controller only resolves the recruiter and renders the result.
    public class RecruiterDashboardService : IRecruiterDashboardService
    {
        private readonly IRecruiterDashboardRepository _dashboardRepo;
        private readonly ICompanyPackageHistoryRepository _packageRepo;
        private readonly IRecruiterRepository _recruiterRepo;

        public RecruiterDashboardService(
            IRecruiterDashboardRepository dashboardRepo,
            ICompanyPackageHistoryRepository packageRepo,
            IRecruiterRepository recruiterRepo)
        {
            _dashboardRepo = dashboardRepo;
            _packageRepo = packageRepo;
            _recruiterRepo = recruiterRepo;
        }

        public async Task<RecruiterDashboard> GetDashboardAsync(int companyId, int recruiterId, string? range)
        {
            var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);

            // Core data.
            var posts = await _dashboardRepo.GetJobPostsAsync(companyId);        // all posts, newest first
            var interviews = await _dashboardRepo.GetInterviewsAsync(companyId);

            var scheduled = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "SCHEDULED" || s == "PENDING"; })
                .ToList();
            var completed = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "FINISHED" || s == "COMPLETED" || s == "CLOSED"; })
                .ToList();

            // Recent posts (top 5 newest) + applicant avatars.
            var recentPosts = posts.Where(j=>j.Status=="APPROVED").Take(5).ToList();
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

            var package = await _packageRepo.GetActivePackageForCompanyAsync(companyId);
            var expiringJobs = await _dashboardRepo.GetExpiringJobsAsync(companyId);
            var recentApps = await _dashboardRepo.GetRecentApplicationsAsync(companyId);
            bool hasPending = await _recruiterRepo.HasPendingVerificationRequestAsync(recruiterId);

            // Missing profile fields.
            var missingFields = new List<string>();
            if (string.IsNullOrEmpty(recruiter.Company?.CompanyLogoUrl)) missingFields.Add("Logo công ty");
            if (string.IsNullOrEmpty(recruiter.Company?.CompanyDescription)) missingFields.Add("Giới thiệu công ty");
            if (string.IsNullOrEmpty(recruiter.Company?.Website)) missingFields.Add("Website");
            if (string.IsNullOrEmpty(recruiter.Company?.Industry)) missingFields.Add("Ngành nghề");
            if (string.IsNullOrEmpty(recruiter.Company?.TaxCode)) missingFields.Add("Mã số thuế");
            if (string.IsNullOrEmpty(recruiter.Company?.BusinessLicenseUrl)) missingFields.Add("Giấy phép kinh doanh");

            // Activity chart series (depends on range).
            var stats = await BuildStatsSeriesAsync(companyId, range, interviews);

            return new RecruiterDashboard
            {
                TotalJobPosts            = posts.Count,
                TotalApplications        = await _dashboardRepo.CountApplicationsAsync(companyId),
                TotalScheduledInterviews = scheduled.Count,
                TotalCompletedInterviews = completed.Count,

                JobPostApplicantCounts = jobApplicantCounts,
                ScheduledInterviews = scheduled,
                CompletedInterviews = completed,

                IsVerified             = recruiter.Company?.IsVerified ?? false,
                HasPendingVerification = hasPending,

                HasActivePackage  = package != null,
                ActivePackageName = package?.Service?.PackageName,
                PostsRemaining    = package?.PostsRemaining ?? 0,
                PostsGranted      = package?.PostsGranted ?? 0,
                PackageExpiry     = package?.EndDate,

                ProfileCompletion    = recruiter.Company?.ProfileCompletion ?? 0,
                MissingProfileFields = missingFields,

                ExpiringJobs       = expiringJobs,
                RecentApplications = recentApps,

                StatsLabels        = stats.Labels,
                StatsApplications  = stats.Applications,
                StatsInterviews    = stats.Interviews,
                StatsRange         = stats.Range
            };
        }

        // Builds the zero-filled activity series for the chart.
        // range: "7"/"30" => daily buckets; "year" => last 12 months by month.
        private async Task<(string Range, List<string> Labels, List<int> Applications, List<int> Interviews)>
            BuildStatsSeriesAsync(int companyId, string? range, List<Interview> interviews)
        {
            string statsRange = (range ?? "30").ToLowerInvariant();
            var labels = new List<string>();
            var applications = new List<int>();
            var interviewSeries = new List<int>();

            if (statsRange == "year")
            {
                // Last 12 months, bucketed by month.
                var firstMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-11);
                var appDates = await _dashboardRepo.GetApplicationDatesAsync(companyId, firstMonth);
                var appByMonth = appDates
                    .GroupBy(d => new DateTime(d.Year, d.Month, 1))
                    .ToDictionary(g => g.Key, g => g.Count());
                var interviewByMonth = interviews
                    .Select(i => { var d = i.CreatedAt ?? i.ScheduledTime; return new DateTime(d.Year, d.Month, 1); })
                    .Where(m => m >= firstMonth)
                    .GroupBy(m => m)
                    .ToDictionary(g => g.Key, g => g.Count());

                for (int i = 0; i < 12; i++)
                {
                    var m = firstMonth.AddMonths(i);
                    labels.Add(m.ToString("MM/yyyy"));
                    applications.Add(appByMonth.TryGetValue(m, out var ac) ? ac : 0);
                    interviewSeries.Add(interviewByMonth.TryGetValue(m, out var ic) ? ic : 0);
                }
            }
            else
            {
                // Daily buckets for the last N days (7 or 30; unknown values normalize to 30).
                int days = statsRange == "7" ? 7 : 30;
                statsRange = days == 7 ? "7" : "30";
                var from = DateTime.Today.AddDays(-(days - 1));
                var appDates = await _dashboardRepo.GetApplicationDatesAsync(companyId, from);
                var appByDay = appDates.GroupBy(d => d.Date).ToDictionary(g => g.Key, g => g.Count());
                var interviewByDay = interviews
                    .Select(i => (i.CreatedAt ?? i.ScheduledTime).Date)
                    .Where(d => d >= from.Date)
                    .GroupBy(d => d)
                    .ToDictionary(g => g.Key, g => g.Count());

                for (int i = 0; i < days; i++)
                {
                    var day = from.Date.AddDays(i);
                    labels.Add(day.ToString("d/M"));
                    applications.Add(appByDay.TryGetValue(day, out var ac) ? ac : 0);
                    interviewSeries.Add(interviewByDay.TryGetValue(day, out var ic) ? ic : 0);
                }
            }

            return (statsRange, labels, applications, interviewSeries);
        }
    }
}

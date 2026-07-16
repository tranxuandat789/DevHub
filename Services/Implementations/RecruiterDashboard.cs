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

        public async Task<RecruiterDashboard> GetDashboardAsync(
            int companyId, 
            int recruiterId, 
            string? jobStatus, 
            string? jobQ, 
            string? jobSort, 
            int jobPage = 1)
        {
            var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);

            // Core data.
            var posts = await _dashboardRepo.GetJobPostsAsync(companyId);        // all posts, newest first
            var interviews = await _dashboardRepo.GetInterviewsAsync(companyId);

            var scheduled = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "SCHEDULED"; })
                .ToList();
            var completed = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "COMPLETED_PENDING"; })
                .ToList();
            var passed = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "PASSED"; })
                .ToList();
            var rejected = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "REJECTED"; })
                .ToList();
            var cancelled = interviews
                .Where(i => { var s = (i.Status ?? "").ToUpper(); return s == "CANCELLED"; })
                .ToList();

            var jobStatsResult = await _dashboardRepo.GetJobStatsAsync(
                companyId: companyId,
                statusFilter: jobStatus,
                keyword: jobQ,
                sortBy: jobSort,
                page: jobPage,
                pageSize: 6
            );

            var jobStatsViewModel = new JobStatsTableViewModel
            {
                Items = jobStatsResult.Items,
                TotalCount = jobStatsResult.TotalCount,
                Page = jobPage,
                PageSize = 6,
                FilterStatus = jobStatus,
                Keyword = jobQ,
                SortBy = jobSort
            };

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

            return new RecruiterDashboard
            {
                TotalJobPosts            = posts.Count,
                TotalApplications        = await _dashboardRepo.CountApplicationsAsync(companyId),
                TotalScheduledInterviews = scheduled.Count,
                TotalCompletedInterviews = completed.Count,

                JobStats = jobStatsViewModel,
                ScheduledInterviews = scheduled,
                CompletedInterviews = completed,
                PassedInterviews = passed,
                RejectedInterviews = rejected,
                CancelledInterviews = cancelled,

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
                RecentApplications = recentApps
            };
        }
    }
}

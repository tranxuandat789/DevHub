//AnhPT-01/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IRecruiterDashboardRepository
{
    Task<List<JobPost>> GetJobPostsAsync(int recruiterId);
    Task<List<Interview>> GetInterviewsAsync(int recruiterId);
    Task<List<ExpiringJobAlert>> GetExpiringJobsAsync(int recruiterId, int withinDays = 7);
    Task<List<RecentApplicationItem>> GetRecentApplicationsAsync(int recruiterId, int take = 6);
    Task<Dictionary<int, List<string>>> GetApplicantAvatarsByJobAsync(List<int> jobIds, int perJob = 3);

    // [#1] AppliedAt timestamps of applications to this recruiter's jobs since fromDate (for the 30-day chart).
    Task<List<DateTime>> GetApplicationDatesAsync(int recruiterId, DateTime fromDate);

    // Real (live) count of applications to all of this recruiter's jobs — matches the applicant list
    // total, instead of summing the denormalized job_post.application_count column.
    Task<int> CountApplicationsAsync(int recruiterId);
}
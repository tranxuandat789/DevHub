//AnhPT-01/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IRecruiterDashboardRepository
{
    Task<List<JobPost>> GetJobPostsAsync(int companyId);
    Task<List<Interview>> GetInterviewsAsync(int companyId);
    Task<List<ExpiringJobAlert>> GetExpiringJobsAsync(int companyId, int withinDays = 7);
    Task<List<RecentApplicationItem>> GetRecentApplicationsAsync(int companyId, int take = 6);
    Task<(List<JobStatsRow> Items, int TotalCount)> GetJobStatsAsync(
        int companyId, 
        string? statusFilter, 
        string? keyword, 
        string? sortBy, 
        int page, 
        int pageSize);

    // Real (live) count of applications to all of this recruiter's jobs — matches the applicant list
    // total, instead of summing the denormalized job_post.application_count column.
    Task<int> CountApplicationsAsync(int companyId);
}

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
}
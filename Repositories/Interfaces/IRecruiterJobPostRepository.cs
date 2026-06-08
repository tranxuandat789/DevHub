using DevHub.Models;

namespace DevHub.Repositories.Interfaces;
public interface IRecruiterJobPostRepository
{
    Task<JobPost> CreateJobPostAndDecrementQuotaAsync(JobPost jobPost, int packageHistoryId);
    Task NotifyModeratorsAsync(string title, string message, string referenceType, int referenceId);
    Task<List<JobPost>> GetJobPostsByRecruiterAsync(int recruiterId);
    Task<JobPost?> GetJobPostForEditAsync(int jobId, int recruiterId);
    Task UpdateJobPostAsync(JobPost source, List<CommonTechnology> techs, string newStatus);
    Task DeleteJobPostAsync(int jobId, int recruiterId);
    Task CloseJobPostAsync(int jobId, int recruiterId);
}
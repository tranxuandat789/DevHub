//AnhPT-02/06/2026
using DevHub.Models;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Interfaces;

public interface IRecruiterJobPostService
{
    Task<int> CreateJobPostAsync(int recruiterId, JobPostCreateViewModel vm);
    Task<(bool CanPost, bool HasActivePackage, int PostsRemaining, int ProfileCompletion)> GetActivePackageInfoAsync(int recruiterId);
    Task<List<JobPost>> GetJobPostsByRecruiterAsync(int recruiterId);
    Task<JobPostManageViewModel> GetManagedJobPostsAsync(int recruiterId, string? keyword, string? status, int page, int pageSize);
    Task<JobPost?> GetJobPostDetailAsync(int recruiterId, int jobId);
    Task<JobPost?> GetEditableJobPostAsync(int recruiterId, int jobId);
    Task<JobPostCreateViewModel?> GetJobPostForRepostAsync(int recruiterId, int jobId);
    Task UpdateJobPostAsync(int recruiterId, int jobId, JobPostCreateViewModel vm);
    Task DeleteJobPostAsync(int recruiterId, int jobId);
    Task CloseJobPostAsync(int recruiterId, int jobId);
}
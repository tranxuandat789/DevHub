//AnhPT-02/06/2026
using DevHub.Models;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Interfaces;

public interface IRecruiterJobPostService
{
    Task<int> CreateJobPostAsync(int recruiterId, JobPostCreateViewModel dto);
    Task<(bool CanPost, bool HasActivePackage, int PostsRemaining, int ProfileCompletion)> GetActivePackageInfoAsync(int recruiterId);
    Task<List<JobPost>> GetJobPostsByRecruiterAsync(int recruiterId);
}
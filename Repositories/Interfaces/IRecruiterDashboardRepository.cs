//AnhPT-01/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IRecruiterDashboardRepository
{
    Task<List<JobPost>> GetJobPostsAsync(int recruiterId);
    Task<List<Interview>> GetInterviewsAsync(int recruiterId);
}
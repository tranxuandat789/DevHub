using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Interfaces;

public interface IRecruiterDashboardService
{
    Task<RecruiterDashboardViewModel> GetDashboardAsync(int recruiterId);
}
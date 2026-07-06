using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IRecruiterDashboardService
{
    // Assembles the full recruiter dashboard (totals, package, profile, charts).
    // range: "7" | "30" | "year" (default "30") — controls the activity chart buckets.
    Task<RecruiterDashboard> GetDashboardAsync(int companyId, int recruiterId, string? range);
}

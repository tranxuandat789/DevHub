// Author: [your-name] - Service interface for public job search
using DevHub.ViewModels.Jobs;

namespace DevHub.Services.Interfaces;

/// Service for the public job search page (Guest + Candidate).
/// Separated from IJobPostService (used by Moderator) to ensure SRP.

public interface IJobSearchService
{

    Task<JobSearchPageViewModel> SearchJobsAsync(JobSearchFilterViewModel filter);

    Task<JobDetailViewModel?> GetJobDetailAsync(int id);
}

using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Jobs;

namespace DevHub.Services.Implementations;

/// IJobSearchService implementation — handles job search business logic:
/// calls Repository, maps Model → ViewModel, calculates pagination.
public class JobSearchService : IJobSearchService
{
    private readonly IJobSearchRepository _repo;

    public JobSearchService(IJobSearchRepository repo)
    {
        _repo = repo;
    }

    /// Search for jobs by filter, load filter options from DB, 
    /// map results to JobSearchPageViewModel.
    public async Task<JobSearchPageViewModel> SearchJobsAsync(JobSearchFilterViewModel filter)
    {
        var (items, totalCount) = await _repo.SearchAsync(filter);
        var workingModels       = await _repo.GetDistinctWorkingModelsAsync();
        var expLevels           = await _repo.GetDistinctExperienceLevelsAsync();

        var jobs = items.Select(j => new JobSearchItemViewModel
        {
            JobId          = j.JobId,
            Title          = j.Title,
            CompanyName    = j.Recruiter.CompanyName,
            CompanyLogoUrl = j.Recruiter.CompanyLogoUrl,
            Location       = j.Location,
            WorkingModel   = j.WorkingModel,
            ExperienceLevel = j.ExperienceLevel,
            SalaryMin      = j.SalaryMin,
            SalaryMax      = j.SalaryMax,
            Deadline       = j.Deadline,
            TechNames      = j.Teches.Select(t => t.TechName).ToList(),
        }).ToList();

        return new JobSearchPageViewModel
        {
            Jobs                   = jobs,
            Filter                 = filter,
            TotalCount             = totalCount,
            TotalPages             = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
            WorkingModelOptions    = workingModels,
            ExperienceLevelOptions = expLevels,
        };
    }
    /// Get details of 1 job. Returns null if not found or status != APPROVED.
    public async Task<JobDetailViewModel?> GetJobDetailAsync(int id)
    {
        var job = await _repo.GetByIdAsync(id);

        if (job is null || job.Status != "APPROVED")
            return null;

        return new JobDetailViewModel
        {
            JobId          = job.JobId,
            Title          = job.Title,
            PositionName   = job.Position.PositionName,
            Location       = job.Location,
            WorkingModel   = job.WorkingModel,
            ExperienceLevel = job.ExperienceLevel,
            SalaryMin      = job.SalaryMin,
            SalaryMax      = job.SalaryMax,
            HiringQuota    = job.HiringQuota,
            Deadline       = job.Deadline,
            Description    = job.Description,
            Requirement    = job.Requirement,
            Benefit        = job.Benefit,
            TechNames      = job.Teches.Select(t => t.TechName).ToList(),
            CompanyName    = job.Recruiter.CompanyName,
            CompanyLogoUrl = job.Recruiter.CompanyLogoUrl,
            CompanyAddress = job.Recruiter.CompanyAddress,
            AverageRating  = job.Recruiter.AverageRating,
            IsVerified     = job.Recruiter.IsVerified ?? false,
        };
    }
}

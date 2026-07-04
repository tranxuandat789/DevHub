using DevHub.Models;
using DevHub.ViewModels.Jobs;

namespace DevHub.Repositories.Interfaces;

public interface IJobSearchRepository
{
    /// Search and paginate APPROVED jobs by filter.
    /// Returns tuple: list of jobs on the current page + total number of jobs found.
    Task<(List<JobPost> Items, int TotalCount)> SearchAsync(JobSearchFilterViewModel filter);

    /// Get 1 job by id
    Task<JobPost?> GetByIdAsync(int id);

    /// Get a list of DISTINCT working_model values in APPROVED jobs.
    Task<List<string>> GetDistinctWorkingModelsAsync();

    /// <summary>Get a list of DISTINCT experience_level values in APPROVED jobs.</summary>
    Task<List<string>> GetDistinctExperienceLevelsAsync();

    /// Top techs có nhiều APPROVED job nhất.
    Task<List<(int TechId, string TechName, int JobCount)>> GetTopTechsAsync(int top);

    /// Top locations có nhiều APPROVED job nhất.
    Task<List<(string Location, int JobCount)>> GetTopLocationsAsync(int top);

    /// Top companies có nhiều APPROVED job nhất.
    Task<List<(int CompanyId, string CompanyName, string? LogoUrl, int JobCount)>> GetTopCompaniesAsync(int top);
}


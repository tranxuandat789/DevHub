using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;
public class JobSearchRepository : IJobSearchRepository
{
    private readonly ItrecruitmentDbContext _context;

    public JobSearchRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    /// Search for APPROVED jobs with filter + server-side pagination.
    /// Order by created_at DESC (newest first).
    public async Task<(List<JobPost> Items, int TotalCount)> SearchAsync(JobSearchFilterViewModel filter)
    {
        var query = _context.JobPosts
            .AsNoTracking()
            .Include(j => j.Recruiter)
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .Where(j => j.Status == "APPROVED");

        // Filter: search by title or skill
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var kw = filter.Keyword.Trim();
            query = query.Where(j =>
                j.Title.Contains(kw) ||
                (j.Skill != null && j.Skill.Contains(kw)));
        }

        // Filter: working model
        if (!string.IsNullOrWhiteSpace(filter.WorkingModel))
            query = query.Where(j => j.WorkingModel == filter.WorkingModel);

        // Filter: experience level
        if (!string.IsNullOrWhiteSpace(filter.ExperienceLevel))
            query = query.Where(j => j.ExperienceLevel == filter.ExperienceLevel);

        // Filter: desired salary
        if (filter.DesiredSalary.HasValue && filter.DesiredSalary.Value > 0)
        {
            var desiredVnd = filter.DesiredSalary.Value * 1_000_000m;
            
            // Find jobs where the desired salary falls within the salary_min and salary_max range
            query = query.Where(j => 
                (j.SalaryMin == null || j.SalaryMin <= desiredVnd) &&
                (j.SalaryMax == null || j.SalaryMax >= desiredVnd) &&
                (j.SalaryMin != null || j.SalaryMax != null)
            );
        }

        // Count total before pagination
        var totalCount = await query.CountAsync();

        // Pagination + order newest first
        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// Get details of 1 job by id, including Recruiter + Position + Teches.
    /// Returns null if not found.
    public async Task<JobPost?> GetByIdAsync(int id)
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Include(j => j.Recruiter)
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .FirstOrDefaultAsync(j => j.JobId == id);
    }

    /// Get a list of DISTINCT working_model values in APPROVED jobs.
    public async Task<List<string>> GetDistinctWorkingModelsAsync()
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.Status == "APPROVED" && j.WorkingModel != null)
            .Select(j => j.WorkingModel!)
            .Distinct()
            .OrderBy(v => v)
            .ToListAsync();
    }

    /// Get a list of DISTINCT experience_level values in APPROVED jobs.
    public async Task<List<string>> GetDistinctExperienceLevelsAsync()
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.Status == "APPROVED" && j.ExperienceLevel != null)
            .Select(j => j.ExperienceLevel!)
            .Distinct()
            .OrderBy(v => v)
            .ToListAsync();
    }

}

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
            .Include(j => j.Company)
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .Include(j => j.Provinces)
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
            query = query.Where(j =>
                (j.SalaryMin == null || j.SalaryMin <= desiredVnd) &&
                (j.SalaryMax == null || j.SalaryMax >= desiredVnd) &&
                (j.SalaryMin != null || j.SalaryMax != null)
            );
        }

        // Quick-filter: kỹ năng
        if (filter.TechId.HasValue)
            query = query.Where(j => j.Teches.Any(t => t.TechId == filter.TechId.Value));

        // Quick-filter: thành phố (giờ qua bảng province nối nhiều-nhiều)
        if (!string.IsNullOrWhiteSpace(filter.FilterLocation))
            query = query.Where(j => j.Provinces.Any(p => p.ProvinceName == filter.FilterLocation));

        // Quick-filter: công ty
        if (filter.RecruiterId.HasValue)
            query = query.Where(j => j.CompanyId == filter.RecruiterId.Value);

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
            .Include(j => j.Company)
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .Include(j => j.Provinces)
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

    /// Top N techs có nhiều APPROVED job nhất (qua job_tech_stack).
    public async Task<List<(int TechId, string TechName, int JobCount)>> GetTopTechsAsync(int top)
    {
        return await _context.CommonTechnologies
            .AsNoTracking()
            .Where(t => t.IsActive == true && t.Jobs.Any(j => j.Status == "APPROVED"))
            .Select(t => new
            {
                t.TechId,
                t.TechName,
                JobCount = t.Jobs.Count(j => j.Status == "APPROVED")
            })
            .OrderByDescending(x => x.JobCount)
            .Take(top)
            .Select(x => ValueTuple.Create(x.TechId, x.TechName, x.JobCount))
            .ToListAsync();
    }

    /// Top N tỉnh/thành có nhiều APPROVED job nhất (qua bảng province nối nhiều-nhiều).
    public async Task<List<(string Location, int JobCount)>> GetTopLocationsAsync(int top)
    {
        return await _context.Provinces
            .AsNoTracking()
            .Select(p => new
            {
                Location = p.ProvinceName,
                JobCount = p.JobPosts.Count(j => j.Status == "APPROVED")
            })
            .Where(x => x.JobCount > 0)
            .OrderByDescending(x => x.JobCount)
            .Take(top)
            .Select(x => ValueTuple.Create(x.Location, x.JobCount))
            .ToListAsync();
    }

    /// Top N companies có nhiều APPROVED job nhất.
    public async Task<List<(int CompanyId, string CompanyName, string? LogoUrl, int JobCount)>> GetTopCompaniesAsync(int top)
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.Status == "APPROVED")
            .GroupBy(j => new { j.CompanyId, j.Company.CompanyName, j.Company.CompanyLogoUrl })
            .Select(g => new
            {
                g.Key.CompanyId,
                g.Key.CompanyName,
                g.Key.CompanyLogoUrl,
                JobCount = g.Count()
            })
            .OrderByDescending(x => x.JobCount)
            .Take(top)
            .Select(x => ValueTuple.Create(x.CompanyId, x.CompanyName, x.CompanyLogoUrl, x.JobCount))
            .ToListAsync();
    }
}


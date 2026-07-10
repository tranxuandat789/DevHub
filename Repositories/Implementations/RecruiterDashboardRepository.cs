//AnhPT-02/06/2026
using DevHub.Repositories.Interfaces;
using DevHub.Data;
using DevHub.Models;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class RecruiterDashboardRepository : IRecruiterDashboardRepository
{
    private readonly ItrecruitmentDbContext _context;

    public RecruiterDashboardRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    // All posts of the company (newest first). Used for both totals and the recent-posts list.
    public async Task<List<JobPost>> GetJobPostsAsync(int companyId)
        => await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

    public async Task<List<Interview>> GetInterviewsAsync(int companyId)
        => await _context.Interviews
            .Include(i => i.Candidate)
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Where(i => i.Application.Job.CompanyId == companyId)
            .OrderBy(i => i.ScheduledTime)
            .ToListAsync();

    // [#5] APPROVED posts whose deadline is within the next N days (not yet passed).
    public async Task<List<ExpiringJobAlert>> GetExpiringJobsAsync(int companyId, int withinDays = 7)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var cutoff = today.AddDays(withinDays);

        var rows = await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.CompanyId == companyId
                     && j.Status != null && j.Status.ToUpper() == "APPROVED"
                     && j.Deadline != null
                     && j.Deadline >= today
                     && j.Deadline <= cutoff)
            .OrderBy(j => j.Deadline)
            .Select(j => new { j.JobId, j.Title, Deadline = j.Deadline!.Value })
            .ToListAsync();

        // Compute DaysLeft in memory (DateOnly.DayNumber doesn't translate to SQL).
        return rows.Select(r => new ExpiringJobAlert
        {
            JobId = r.JobId,
            Title = r.Title,
            Deadline = r.Deadline,
            DaysLeft = r.Deadline.DayNumber - today.DayNumber
        }).ToList();
    }

    // [#6] Most recent applications to this company's jobs.
    public async Task<List<RecentApplicationItem>> GetRecentApplicationsAsync(int companyId, int take = 6)
        => await _context.Applications
            .AsNoTracking()
            .Where(a => a.Job.CompanyId == companyId)
            .OrderByDescending(a => a.AppliedAt)
            .Take(take)
            .Select(a => new RecentApplicationItem
            {
                ApplicationId = a.ApplicationId,
                CandidateName = a.Candidate.FullName,
                AvatarUrl = a.Candidate.ImageUrl,
                JobTitle = a.Job.Title,
                AppliedAt = a.AppliedAt,
                ApplicationStatus = a.Status ?? "PENDING"
            })
            .ToListAsync();

    public async Task<(List<JobStatsRow> Items, int TotalCount)> GetJobStatsAsync(
        int companyId, 
        string? statusFilter, 
        string? keyword, 
        string? sortBy, 
        int page, 
        int pageSize)
    {
        var query = _context.JobPosts.AsNoTracking().Where(j => j.CompanyId == companyId);

        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "ALL")
        {
            query = query.Where(j => j.Status == statusFilter);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim().ToLower();
            // Reusing title search logic: remove extra spaces if needed, but Contains is fine.
            query = query.Where(j => j.Title != null && j.Title.ToLower().Contains(k));
        }

        var totalCount = await query.CountAsync();

        // Project with correlated subqueries for counts
        var statsQuery = query.Select(j => new JobStatsRow
        {
            JobId = j.JobId,
            Title = j.Title ?? "",
            Status = j.Status ?? "PENDING",
            CreatedAt = j.CreatedAt,
            PendingCount = j.Applications.Count(a => a.Status == "PENDING"),
            // Match RecruiterApplicationRepository tab-grouping for Approved
            ApprovedCount = j.Applications.Count(a => a.Status == "APPROVED" || a.Status == "FINISHED"),
            // Hired definition
            HiredCount = j.Applications.Count(a => a.Status == "HIRED"),
            TotalApplicationCount = j.Applications.Count()
        });

        // Sorting
        statsQuery = sortBy switch
        {
            "oldest" => statsQuery.OrderBy(s => s.CreatedAt),
            "newest" => statsQuery.OrderByDescending(s => s.CreatedAt),
            "name_az" => statsQuery.OrderBy(s => s.Title),
            "pending_desc" => statsQuery.OrderByDescending(s => s.PendingCount),
            _ => statsQuery.OrderByDescending(s => s.PendingCount) // default
        };

        var items = await statsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    // Live count of all applications to the company's jobs (matches the applicant-list total).
    public async Task<int> CountApplicationsAsync(int companyId)
        => await _context.Applications
            .AsNoTracking()
            .CountAsync(a => a.Job.CompanyId == companyId);
}

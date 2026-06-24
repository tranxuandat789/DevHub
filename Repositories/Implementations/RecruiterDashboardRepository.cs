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

    // All posts of the recruiter (newest first). Used for both totals and the recent-posts list.
    public async Task<List<JobPost>> GetJobPostsAsync(int recruiterId)
        => await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.RecruiterId == recruiterId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

    public async Task<List<Interview>> GetInterviewsAsync(int recruiterId)
        => await _context.Interviews
            .Include(i => i.Candidate)
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Where(i => i.RecruiterId == recruiterId)
            .OrderBy(i => i.ScheduledTime)
            .ToListAsync();

    // [#5] APPROVED posts whose deadline is within the next N days (not yet passed).
    public async Task<List<ExpiringJobAlert>> GetExpiringJobsAsync(int recruiterId, int withinDays = 7)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var cutoff = today.AddDays(withinDays);

        var rows = await _context.JobPosts
            .AsNoTracking()
            .Where(j => j.RecruiterId == recruiterId
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

    // [#6] Most recent applications to this recruiter's jobs.
    public async Task<List<RecentApplicationItem>> GetRecentApplicationsAsync(int recruiterId, int take = 6)
        => await _context.Applications
            .AsNoTracking()
            .Where(a => a.Job.RecruiterId == recruiterId)
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

    // Recent applicant avatars for each job (avatar stack). Returns resolved URLs (ImageUrl or ui-avatars fallback).
    public async Task<Dictionary<int, List<string>>> GetApplicantAvatarsByJobAsync(List<int> jobIds, int perJob = 3)
    {
        if (jobIds == null || jobIds.Count == 0)
            return new Dictionary<int, List<string>>();

        var apps = await _context.Applications
            .AsNoTracking()
            .Where(a => jobIds.Contains(a.JobId))
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new { a.JobId, a.Candidate.ImageUrl, a.Candidate.FullName })
            .ToListAsync();

        return apps
            .GroupBy(a => a.JobId)
            .ToDictionary(
                g => g.Key,
                g => g.Take(perJob).Select(x =>
                    !string.IsNullOrEmpty(x.ImageUrl)
                        ? x.ImageUrl!
                        : $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(x.FullName)}&background=4640DE&color=fff&size=64"
                ).ToList());
    }

    // [#1] AppliedAt timestamps for the recruiter's jobs since fromDate (bucketed into the 30-day chart by the controller).
    public async Task<List<DateTime>> GetApplicationDatesAsync(int recruiterId, DateTime fromDate)
        => await _context.Applications
            .AsNoTracking()
            .Where(a => a.Job.RecruiterId == recruiterId
                     && a.AppliedAt != null
                     && a.AppliedAt >= fromDate)
            .Select(a => a.AppliedAt!.Value)
            .ToListAsync();
}
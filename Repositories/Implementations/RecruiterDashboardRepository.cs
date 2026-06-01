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

    public async Task<List<JobPost>> GetJobPostsAsync(int recruiterId)
        => await _context.JobPosts
            .Where(j => j.RecruiterId == recruiterId)
            .OrderByDescending(j => j.CreatedAt)
            .Take(10)
            .ToListAsync();

    public async Task<List<Interview>> GetInterviewsAsync(int recruiterId)
        => await _context.Interviews
            .Include(i => i.Candidate)
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Where(i => i.RecruiterId == recruiterId)
            .OrderBy(i => i.ScheduledTime)
            .ToListAsync();
}
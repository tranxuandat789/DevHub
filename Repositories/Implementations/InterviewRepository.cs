using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class InterviewRepository : IInterviewRepository
{
    private readonly ItrecruitmentDbContext _context;

    public InterviewRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<Application?> GetApplicationForInterviewAsync(int applicationId)
    {
        return await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate).ThenInclude(c => c.CandidateNavigation)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
    }

    public async Task<Interview> AddInterviewAsync(Interview interview)
    {
        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();
        return interview;
    }

    public async Task<Interview?> GetInterviewForRecruiterAsync(int interviewId, int recruiterId)
    {
        var companyId = await _context.Recruiters
            .Where(r => r.RecruiterId == recruiterId)
            .Select(r => r.CompanyId)
            .FirstOrDefaultAsync();

        return await _context.Interviews
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
            .FirstOrDefaultAsync(i => i.InterviewId == interviewId && i.Application != null && i.Application.Job != null && i.Application.Job.CompanyId == companyId);
    }

    public async Task UpdateAsync(Interview interview)
    {
        _context.Interviews.Update(interview);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Interview>> GetPastScheduledInterviewsAsync()
    {
        var now = System.DateTime.Now;
        return await _context.Interviews
            .Where(i => (i.Status == "scheduled" || i.Status == "confirmed") && i.ScheduledTime < now)
            .ToListAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Interview> interviews)
    {
        _context.Interviews.UpdateRange(interviews);
        await _context.SaveChangesAsync();
    }
}

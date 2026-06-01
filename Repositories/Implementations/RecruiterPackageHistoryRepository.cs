using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class RecruiterPackageHistoryRepository : IRecruiterPackageHistoryRepository
{
    private readonly ItrecruitmentDbContext _context;
    public RecruiterPackageHistoryRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }
    public async Task<RecruiterPackageHistory?> GetActivePackageForRecruiterAsync(int recruiterId)
    {
        var now = DateTime.UtcNow;
        return await _context.RecruiterPackageHistories
            .Include(r => r.Service)
            .Where(r => r.RecruiterId == recruiterId && r.IsActive == true && r.PostsRemaining > 0 && (r.EndDate == null || r.EndDate >= now))
            .OrderByDescending(r => r.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task DecrementPostsRemainingAsync(int packageHistoryId)
    {
        var p = await _context.RecruiterPackageHistories.FindAsync(packageHistoryId);
        if (p == null) return;
        p.PostsRemaining = Math.Max(0, p.PostsRemaining - 1);
        _context.RecruiterPackageHistories.Update(p);
        await _context.SaveChangesAsync();
    }
}

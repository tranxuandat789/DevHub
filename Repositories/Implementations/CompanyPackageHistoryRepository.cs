//AnhPT-02/06/2026
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CompanyPackageHistoryRepository : ICompanyPackageHistoryRepository
{
    private readonly ItrecruitmentDbContext _context;
    public CompanyPackageHistoryRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }
    public async Task<CompanyPackageHistory?> GetActivePackageForCompanyAsync(int companyId)
    {
        var now = DateTime.UtcNow;
        return await _context.CompanyPackageHistories
            .Include(r => r.Service)
            .Where(r => r.CompanyId == companyId && r.IsActive == true && r.PostsRemaining > 0 && (r.EndDate == null || r.EndDate >= now))
            .OrderByDescending(r => r.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task DecrementPostsRemainingAsync(int packageHistoryId)
    {
        var p = await _context.CompanyPackageHistories.FindAsync(packageHistoryId);
        if (p == null) return;
        p.PostsRemaining = Math.Max(0, p.PostsRemaining - 1);
        _context.CompanyPackageHistories.Update(p);
        await _context.SaveChangesAsync();
    }
}

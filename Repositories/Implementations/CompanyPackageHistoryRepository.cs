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
        var activePackages = await _context.CompanyPackageHistories
            .Include(r => r.Service)
            .Where(r => r.CompanyId == companyId && r.IsActive == true)
            .ToListAsync();

        var now = DateTime.UtcNow;
        bool changed = false;

        foreach (var pkg in activePackages)
        {
            if (pkg.PostsRemaining <= 0 || (pkg.EndDate.HasValue && pkg.EndDate.Value < now))
            {
                pkg.IsActive = false;
                changed = true;
            }
        }

        if (changed)
        {
            await _context.SaveChangesAsync();
        }

        return activePackages.Where(h => h.IsActive == true)
                             .OrderByDescending(r => r.EndDate)
                             .FirstOrDefault();
    }

    public async Task DecrementPostsRemainingAsync(int packageHistoryId)
    {
        var p = await _context.CompanyPackageHistories.FindAsync(packageHistoryId);
        if (p == null) return;
        p.PostsRemaining = Math.Max(0, p.PostsRemaining - 1);
        if (p.PostsRemaining == 0)
        {
            p.IsActive = false;
        }
        _context.CompanyPackageHistories.Update(p);
        await _context.SaveChangesAsync();
    }
}

using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DevHub.Repositories.Implementations;

public class RecruiterRepository : IRecruiterRepository
{
    private readonly ItrecruitmentDbContext _db;

    public RecruiterRepository(ItrecruitmentDbContext db) => _db = db;

    public async Task<Recruiter> AddAsync(Recruiter recruiter)
    {
        _db.Recruiters.Add(recruiter);
        await _db.SaveChangesAsync();
        return recruiter;
    }

    public Task UpdateCompanyLogoAsync(int recruiterId, string logoUrl)
        => _db.Recruiters
              .Where(r => r.RecruiterId == recruiterId)
              .ExecuteUpdateAsync(s => s.SetProperty(r => r.CompanyLogoUrl, logoUrl));
}

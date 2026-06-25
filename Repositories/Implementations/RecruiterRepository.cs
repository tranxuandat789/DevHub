//AnhPT-02/06/2026
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

    public async Task<Recruiter> GetProfileAsync(int recruiterId)
        {
            return await _db.Recruiters
                .Include(r => r.RecruiterNavigation) //retirve email from user_account
                .FirstOrDefaultAsync(r => r.RecruiterId == recruiterId);
        }

    public async Task UpdateProfileAsync(Recruiter recruiter)
    {
        await _db.Recruiters
            .Where(r => r.RecruiterId == recruiter.RecruiterId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.FullName, recruiter.FullName)
                .SetProperty(r => r.Position, recruiter.Position)
                .SetProperty(r => r.Phone, recruiter.Phone)
                .SetProperty(r => r.CompanyName, recruiter.CompanyName)
                .SetProperty(r => r.CompanyAddress, recruiter.CompanyAddress)
                .SetProperty(r => r.CompanyLogoUrl, recruiter.CompanyLogoUrl)
                .SetProperty(r => r.CompanyDescription, recruiter.CompanyDescription)
                .SetProperty(r => r.Website, recruiter.Website)
                .SetProperty(r => r.Industry, recruiter.Industry)
                .SetProperty(r => r.TaxCode, recruiter.TaxCode)
                .SetProperty(r => r.BusinessLicenseUrl, recruiter.BusinessLicenseUrl)
                .SetProperty(r => r.AdditionalDocumentsUrl, recruiter.AdditionalDocumentsUrl)
                .SetProperty(r => r.ProfileCompletion, recruiter.ProfileCompletion)
            );
    }

    public async Task<Recruiter> GetProfileForUpdateAsync(int recruiterId)
    {
        return await _db.Recruiters
            .FirstOrDefaultAsync(r => r.RecruiterId == recruiterId);
    }

    public async Task<bool> CheckTaxCodeExistAsync(string taxCode, int excludeRecruiterId)
    {
        if (string.IsNullOrWhiteSpace(taxCode))
            return false;

        return await _db.Recruiters
            .AnyAsync(r => r.TaxCode == taxCode && r.RecruiterId != excludeRecruiterId);
    }

    public async Task CreateVerificationRequestAsync(int recruiterId, string details)
    {
        var log = new AuditLog
        {
            UserId = recruiterId,
            UserType = "RECRUITER",
            Action = "VerificationRequest",
            EntityType = "recruiter_profile",
            EntityId = recruiterId,
            NewValue = details?.Length > 4000 ? details[..4000] : details,
            CreatedAt = DateTime.Now
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> HasPendingVerificationRequestAsync(int recruiterId)
    {
        return await _db.AuditLogs.AnyAsync(a => 
            a.Action == "VerificationRequest" && 
            a.EntityType == "recruiter_profile" && 
            a.EntityId == recruiterId && 
            a.OldValue == null);
    }
}          

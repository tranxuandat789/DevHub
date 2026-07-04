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

    public async Task UpdateCompanyLogoAsync(int recruiterId, string logoUrl)
    {
        var recruiter = await _db.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == recruiterId);
        if (recruiter != null && recruiter.CompanyId.HasValue)
        {
            await _db.Companies
                  .Where(c => c.CompanyId == recruiter.CompanyId.Value)
                  .ExecuteUpdateAsync(s => s.SetProperty(c => c.CompanyLogoUrl, logoUrl));
        }
    }

    public async Task<Recruiter> GetProfileAsync(int recruiterId)
        {
            return (await _db.Recruiters
                .Include(r => r.RecruiterNavigation) //retirve email from user_account
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.RecruiterId == recruiterId))!;
        }

    public async Task UpdateProfileAsync(Recruiter recruiter)
    {
        await _db.Recruiters
            .Where(r => r.RecruiterId == recruiter.RecruiterId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.FullName, recruiter.FullName)
                .SetProperty(r => r.Position, recruiter.Position)
                .SetProperty(r => r.Phone, recruiter.Phone)
            );

        if (recruiter.CompanyId.HasValue && recruiter.Company != null)
        {
            await _db.Companies
                .Where(c => c.CompanyId == recruiter.CompanyId.Value)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.CompanyName, recruiter.Company.CompanyName)
                    .SetProperty(c => c.CompanyAddress, recruiter.Company.CompanyAddress)
                    .SetProperty(c => c.CompanyLogoUrl, recruiter.Company.CompanyLogoUrl)
                    .SetProperty(c => c.CompanyDescription, recruiter.Company.CompanyDescription)
                    .SetProperty(c => c.Website, recruiter.Company.Website)
                    .SetProperty(c => c.Industry, recruiter.Company.Industry)
                    .SetProperty(c => c.TaxCode, recruiter.Company.TaxCode)
                    .SetProperty(c => c.BusinessLicenseUrl, recruiter.Company.BusinessLicenseUrl)
                    .SetProperty(c => c.AdditionalDocumentsUrl, recruiter.Company.AdditionalDocumentsUrl)
                    .SetProperty(c => c.ProfileCompletion, recruiter.Company.ProfileCompletion)
                );
        }
    }

    public async Task<Recruiter> GetProfileForUpdateAsync(int recruiterId)
    {
        return (await _db.Recruiters
            .Include(r => r.Company)
            .FirstOrDefaultAsync(r => r.RecruiterId == recruiterId))!;
    }

    public async Task<bool> CheckTaxCodeExistAsync(string taxCode, int excludeRecruiterId)
    {
        if (string.IsNullOrWhiteSpace(taxCode))
            return false;

        return await _db.Companies
            .AnyAsync(c => c.TaxCode == taxCode && c.CompanyId != excludeRecruiterId);
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

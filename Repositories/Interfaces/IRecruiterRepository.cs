//AnhPT-01/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IRecruiterRepository
{
    Task<Recruiter> AddAsync(Recruiter recruiter);
    Task UpdateCompanyLogoAsync(int recruiterId, string logoUrl);
    Task<Recruiter> GetProfileAsync(int recruiterId);
    Task<Recruiter> GetProfileForUpdateAsync(int recruiterId);
    Task UpdateProfileAsync(Recruiter recruiter);
    Task CreateVerificationRequestAsync(int recruiterId, string details);
    Task<bool> HasPendingVerificationRequestAsync(int recruiterId);
    Task<bool> CheckTaxCodeExistAsync(string taxCode, int excludeRecruiterId);
    Task<List<Recruiter>> GetRecruitersByCompanyIdAsync(int companyId);
}

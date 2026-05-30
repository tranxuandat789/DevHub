using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IRecruiterRepository
{
    Task<Recruiter> AddAsync(Recruiter recruiter);
    Task UpdateCompanyLogoAsync(int recruiterId, string logoUrl);
}

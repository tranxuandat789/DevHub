//03/06/2026 DatTX
namespace DevHub.Services.Interfaces;

public interface ICandidateService
{
    Task<DevHub.Models.Candidate?> GetCandidateByIdAsync(int candidateId);

    Task UpdateProfileAsync(int candidateId, string fullName, string? phone, DateOnly? birthdate, string? gender, string? address, string? socialMediaUrl, decimal? expectedSalaryMin, decimal? expectedSalaryMax, string? preferredLocation, int? experienceYears, bool cvSearchable);

    Task<int> CalculateAndSaveCompletionAsync(int candidateId);
}

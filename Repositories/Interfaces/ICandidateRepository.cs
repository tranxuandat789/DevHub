using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICandidateRepository
{
    Task<Candidate> AddAsync(Candidate candidate);
    Task UpdateAvatarAsync(int candidateId, string avatarUrl);
    Task<Candidate?> GetByIdWithDetailsAsync(int candidateId);
    Task UpdateProfileAsync(int candidateId, string fullName, string? phone, DateOnly? birthdate, string? gender, string? address, string? socialMediaUrl, decimal? expectedSalaryMin, decimal? expectedSalaryMax, string? preferredLocation, int? experienceYears, bool cvSearchable);
    Task UpdateProfileCompletionAsync(int candidateId, int percent);
}

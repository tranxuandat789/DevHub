//03/06/2026 DatTX
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CandidateRepository : ICandidateRepository
{
    private readonly ItrecruitmentDbContext _db;

    public CandidateRepository(ItrecruitmentDbContext db) => _db = db;

    public async Task<Candidate> AddAsync(Candidate candidate)
    {
        _db.Candidates.Add(candidate);
        await _db.SaveChangesAsync();
        return candidate;
    }
    // updates the avatar URL for a specific candidate in the database
    public Task UpdateAvatarAsync(int candidateId, string avatarUrl)
        => _db.Candidates
              .Where(c => c.CandidateId == candidateId)
              .ExecuteUpdateAsync(s => s.SetProperty(c => c.ImageUrl, avatarUrl));
    //queries the candidate by id  and get details
    public async Task<Candidate?> GetByIdWithDetailsAsync(int candidateId)
    {
        return await _db.Candidates
            .Include(c => c.CandidateNavigation)
            .Include(c => c.CandidateSkills)
            .Include(c => c.Cvs)
            .FirstOrDefaultAsync(c => c.CandidateId == candidateId);
    }
    // updates the candidate profile with the provided details
    public async Task UpdateProfileAsync(int candidateId, string fullName, string? phone, DateOnly? birthdate, string? gender, string? address, string? socialMediaUrl, decimal? expectedSalaryMin, decimal? expectedSalaryMax, string? preferredLocation, int? experienceYears, bool cvSearchable)
    {
        await _db.Candidates
            .Where(c => c.CandidateId == candidateId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.FullName, fullName)
                .SetProperty(c => c.Phone, phone)
                .SetProperty(c => c.Birthdate, birthdate)
                .SetProperty(c => c.Gender, gender)
                .SetProperty(c => c.Address, address)
                .SetProperty(c => c.SocialMediaUrl, socialMediaUrl)
                .SetProperty(c => c.ExpectedSalaryMin, expectedSalaryMin)
                .SetProperty(c => c.ExpectedSalaryMax, expectedSalaryMax)
                .SetProperty(c => c.PreferredLocation, preferredLocation)
                .SetProperty(c => c.ExperienceYears, experienceYears)
                .SetProperty(c => c.CvSearchable, cvSearchable));
    }
    // updates the profile completion percentage for a candidate
    public async Task UpdateProfileCompletionAsync(int candidateId, int percent)
    {
        await _db.Candidates
            .Where(c => c.CandidateId == candidateId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ProfileCompletion, percent));
    }

}

//03/06/2026 DatTX
using DevHub.Repositories.Interfaces;
using DevHub.Data;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CvRepository : ICvRepository
{
    private readonly ItrecruitmentDbContext _db;
    public CvRepository(ItrecruitmentDbContext db) => _db = db;
    // Retrieves the default CV for a specific candidate by their ID.
    public async Task<DevHub.Models.Cv?> GetDefaultByCandidateIdAsync(int candidateId)
    {
        return await _db.Cvs.FirstOrDefaultAsync(c => c.CandidateId == candidateId && c.IsDefault == true);
    }

    public async Task UpsertCvFileAsync(int candidateId, string cvUrl)
    {
        var cv = await GetDefaultByCandidateIdAsync(candidateId);
        if (cv == null)
        {
            // if there is no default CV for the candidate, create a new one with the provided CV URL
            cv = new DevHub.Models.Cv
            {
                CandidateId = candidateId,
                Title = "CV Mặc định",
                CvUrl = cvUrl,
                IsDefault = true,
                CreatedAt = DateTime.Now
            };
            _db.Cvs.Add(cv);
        }
        else
        {
            // if there is an existing default CV, update its CV URL with the new one
            cv.CvUrl = cvUrl;
            cv.UpdatedAt = DateTime.Now;
        }
        await _db.SaveChangesAsync(); 
    }

    // for each of the update methods, we first retrieve the default CV for the candidate. 

    public async Task UpdateEducationAsync(int candidateId, string? education)
    {
        var cv = await GetOrCreateDefaultCvAsync(candidateId);
        cv.Education = education;
        cv.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task UpdateExperienceAsync(int candidateId, string? experience)
    {
        var cv = await GetOrCreateDefaultCvAsync(candidateId);
        cv.Experience = experience;
        cv.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task UpdateLanguagesAsync(int candidateId, string? languages)
    {
        var cv = await GetOrCreateDefaultCvAsync(candidateId);
        cv.Languages = languages;
        cv.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    public async Task UpdateSkillsAsync(int candidateId, string? skills)
    {
        var cv = await GetOrCreateDefaultCvAsync(candidateId);
        cv.Skills = skills;
        cv.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
    }

    private async Task<DevHub.Models.Cv> GetOrCreateDefaultCvAsync(int candidateId)
    {
        var cv = await GetDefaultByCandidateIdAsync(candidateId);
        if (cv == null)
        {
            cv = new DevHub.Models.Cv
            {
                CandidateId = candidateId,
                Title = "CV Mặc định",
                IsDefault = true,
                CreatedAt = DateTime.Now
            };
            _db.Cvs.Add(cv);
            await _db.SaveChangesAsync();
        }
        return cv;
    }
}

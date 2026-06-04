//03/06/2026 DatTX
namespace DevHub.Repositories.Interfaces;

public interface ICvRepository
{
    Task<DevHub.Models.Cv?> GetDefaultByCandidateIdAsync(int candidateId);
    Task UpsertCvFileAsync(int candidateId, string cvUrl);
    Task UpdateEducationAsync(int candidateId, string? education);
    Task UpdateExperienceAsync(int candidateId, string? experience);
    Task UpdateLanguagesAsync(int candidateId, string? languages);
    Task UpdateSkillsAsync(int candidateId, string? skills);
}

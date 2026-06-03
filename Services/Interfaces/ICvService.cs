namespace DevHub.Services.Interfaces;

public interface ICvService
{
    Task<DevHub.Models.Cv?> GetCvByCandidateIdAsync(int candidateId);
    Task<string> UploadCvFileAsync(int candidateId, IFormFile file, string webRootPath);
    Task UpdateEducationAsync(int candidateId, string? educationJson);
    Task UpdateExperienceAsync(int candidateId, string? experienceJson);
    Task UpdateLanguagesAsync(int candidateId, string? languagesJson);
    Task UpdateSkillsAsync(int candidateId, string? skillsJson);
    Task AddOrUpdateSkillAsync(int candidateId, string skillName, string proficiency);
}

using DevHub.Services.Interfaces;
using DevHub.Repositories.Interfaces;
using DevHub.Models;
namespace DevHub.Services.Implementations;

public class CvService : ICvService
{ 
    private readonly ICvRepository _cvRepository;
    public CvService(ICvRepository cvRepository)
    {
        _cvRepository = cvRepository;
    }
    // retrieves the CV information for a specific candidate by their ID
    public async Task<Cv?> GetCvByCandidateIdAsync(int candidateId)
    {
        return await _cvRepository.GetDefaultByCandidateIdAsync(candidateId);
    }
    // handles the uploading of a CV file for a candidate, including saving the file to the server, updating the database record with the new file URL, and deleting any existing CV file if it exists
    public async Task<string> UploadCvFileAsync(int candidateId, IFormFile file, string webRootPath) {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Vui lòng chọn file CV!");
        if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Dung lượng file CV không được vượt quá 5MB!");
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".pdf" && extension != ".docx")
            throw new ArgumentException("Hệ thống chỉ chấp nhận file định dạng .pdf hoặc .docx!");

        var currentCv = await _cvRepository.GetDefaultByCandidateIdAsync(candidateId);
        // ensure the upload folder exists
        string uploadFolder = Path.Combine(webRootPath, "uploads", "cvs");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        if (currentCv != null && !string.IsNullOrEmpty(currentCv.CvUrl))
        {
            // if there is an existing CV file, delete it from the server before uploading the new one
            string oldFilePath = Path.Combine(webRootPath, currentCv.CvUrl.TrimStart('/'));
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
        }
        // generate a unique file name for the uploaded CV to avoid conflicts
        string uniqueFileName = $"{candidateId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string newFilePath = Path.Combine(uploadFolder, uniqueFileName);
        // use FileStream to save the uploaded file to the server
        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        // update the candidate's CV record in the database with the new file URL
        string newCvUrl = $"/uploads/cvs/{uniqueFileName}";
        await _cvRepository.UpsertCvFileAsync(candidateId, newCvUrl);
        return newCvUrl; 
    }
    //Update the education, experience, and languages sections of a candidate's CV with the provided JSON data.
    public async Task UpdateEducationAsync(int candidateId, string? educationJson)
    {
        await _cvRepository.UpdateEducationAsync(candidateId, educationJson);
    }
    public async Task UpdateExperienceAsync(int candidateId, string? experienceJson)
    {
        await _cvRepository.UpdateExperienceAsync(candidateId, experienceJson);
    }
    public async Task UpdateLanguagesAsync(int candidateId, string? languagesJson)
    {
        await _cvRepository.UpdateLanguagesAsync(candidateId, languagesJson);
    }
    public async Task UpdateSkillsAsync(int candidateId, string? skillsJson)
    {
        await _cvRepository.UpdateSkillsAsync(candidateId, skillsJson);
    }
    // adds a new skill to the candidate's CV or updates the proficiency level of an existing skill..
    public async Task AddOrUpdateSkillAsync(int candidateId, string skillName, string proficiency)
    {
        var cv = await GetCvByCandidateIdAsync(candidateId);
        var skillsList = new List<DevHub.DTOs.Candidate.SkillDto>();
        if (cv != null && !string.IsNullOrEmpty(cv.Skills))
        {
            try { skillsList = System.Text.Json.JsonSerializer.Deserialize<List<DevHub.DTOs.Candidate.SkillDto>>(cv.Skills) ?? new(); } catch {}
        }
        var existingSkill = skillsList.FirstOrDefault(s => s.SkillName == skillName);
        if (existingSkill != null)
        {
            skillsList.Remove(existingSkill);
        }
        skillsList.Add(new DevHub.DTOs.Candidate.SkillDto { SkillName = skillName, Proficiency = proficiency });
        var json = System.Text.Json.JsonSerializer.Serialize(skillsList);
        await _cvRepository.UpdateSkillsAsync(candidateId, json);
    }
}

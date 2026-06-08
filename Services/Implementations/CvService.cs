//03/06/2026 DatTX
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
    // handles the uploading of a CV file for a candidate
    public async Task<string> UploadCvFileAsync(int candidateId, IFormFile file, string webRootPath)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Vui lòng chọn file CV!");
        if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Dung lượng file CV không được vượt quá 5MB!");
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension != ".pdf" && extension != ".docx")
            throw new ArgumentException("Hệ thống chỉ chấp nhận file định dạng .pdf hoặc .docx!");

        var currentCv = await _cvRepository.GetDefaultByCandidateIdAsync(candidateId);
        string uploadFolder = Path.Combine(webRootPath, "uploads", "cvs");
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        if (currentCv != null && !string.IsNullOrEmpty(currentCv.CvUrl))
        {
            string oldFilePath = Path.Combine(webRootPath, currentCv.CvUrl.TrimStart('/'));
            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);
        }

        string uniqueFileName = $"{candidateId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string newFilePath = Path.Combine(uploadFolder, uniqueFileName);
        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        string newCvUrl = $"/uploads/cvs/{uniqueFileName}";
        await _cvRepository.UpsertCvFileAsync(candidateId, newCvUrl);
        return newCvUrl;
    }
}

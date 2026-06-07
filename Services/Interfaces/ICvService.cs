//03/06/2026 DatTX
namespace DevHub.Services.Interfaces;

public interface ICvService
{
    Task<DevHub.Models.Cv?> GetCvByCandidateIdAsync(int candidateId);
    Task<string> UploadCvFileAsync(int candidateId, IFormFile file, string webRootPath);
}


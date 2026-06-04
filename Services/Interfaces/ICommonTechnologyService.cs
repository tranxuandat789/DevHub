using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface ICommonTechnologyService
{
    Task<List<CommonTechnology>> GetAllTechsAsync();
    Task<CommonTechnology?> GetTechByIdAsync(int id);
    Task UpdateTechAsync(CommonTechnology tech);
    Task AddTechAsync(CommonTechnology tech);
    Task<bool> ToggleStatusAsync(int id);
}

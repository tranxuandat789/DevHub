using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IProvinceService
{
    Task<List<Province>> GetAllProvincesAsync();
    Task<Province?> GetProvinceByIdAsync(int id);
    Task AddProvinceAsync(Province province);
    Task UpdateProvinceAsync(Province province);
    Task<bool> ToggleStatusAsync(int id);
}

//4/6/2026
// author: Hoang Minh Kien

namespace DevHub.Services.Interfaces;

public interface ICommonJobPositionService
{
    Task<List<DevHub.Models.CommonJobPosition>> GetAllPositionsAsync();
    Task<DevHub.Models.CommonJobPosition?> GetPositionByIdAsync(int id);
    Task UpdatePositionAsync(DevHub.Models.CommonJobPosition position);
    Task ToggleStatusAsync(int id);
}

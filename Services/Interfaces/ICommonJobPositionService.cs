namespace DevHub.Services.Interfaces;

public interface ICommonJobPositionService
{
    Task<List<DevHub.Models.CommonJobPosition>> GetAllPositionsAsync();
    Task<DevHub.Models.CommonJobPosition?> GetPositionByIdAsync(int id);
    Task UpdatePositionAsync(DevHub.Models.CommonJobPosition position);
    Task AddPositionAsync(DevHub.Models.CommonJobPosition position);
    Task<bool> ToggleStatusAsync(int id);
}

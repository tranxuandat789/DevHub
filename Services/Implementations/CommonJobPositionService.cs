using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class CommonJobPositionService : ICommonJobPositionService
{
    private readonly DevHub.Repositories.Interfaces.ICommonJobPositionRepository _repository;

    public CommonJobPositionService(DevHub.Repositories.Interfaces.ICommonJobPositionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DevHub.Models.CommonJobPosition>> GetAllPositionsAsync()
    {
        var positions = await _repository.GetAllAsync();
        return positions.OrderByDescending(p => p.PositionId).ToList();
    }

    public async Task<DevHub.Models.CommonJobPosition?> GetPositionByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task UpdatePositionAsync(DevHub.Models.CommonJobPosition position)
    {
        await _repository.UpdateAsync(position);
    }

    public async Task AddPositionAsync(DevHub.Models.CommonJobPosition position)
    {
        await _repository.AddAsync(position);
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var position = await _repository.GetByIdAsync(id);
        if (position == null) return false;

        position.IsActive = !(position.IsActive ?? true);
        await _repository.UpdateAsync(position);
        return true;
    }
}

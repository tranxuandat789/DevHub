//4/6/2026
// author: Hoang Minh Kien



using DevHub.Services.Interfaces;

using DevHub.Repositories.Interfaces;
using DevHub.Models;

namespace DevHub.Services.Implementations;

public class CommonJobPositionService : ICommonJobPositionService
{
    private readonly ICommonJobPositionRepository _repository;

    public CommonJobPositionService(ICommonJobPositionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CommonJobPosition>> GetAllPositionsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<CommonJobPosition?> GetPositionByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task UpdatePositionAsync(CommonJobPosition position)
    {
        await _repository.UpdateAsync(position);
    }

    public async Task ToggleStatusAsync(int id)
    {
        var position = await _repository.GetByIdAsync(id);
        if (position != null)
        {
            position.IsActive = !(position.IsActive ?? true);
            await _repository.UpdateAsync(position);
        }
    }
}

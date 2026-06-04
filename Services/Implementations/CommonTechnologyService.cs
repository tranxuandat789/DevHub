using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class CommonTechnologyService : ICommonTechnologyService
{
    private readonly ICommonTechnologyRepository _repository;

    public CommonTechnologyService(ICommonTechnologyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CommonTechnology>> GetAllTechsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<CommonTechnology?> GetTechByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task UpdateTechAsync(CommonTechnology tech)
    {
        await _repository.UpdateAsync(tech);
    }

    public async Task AddTechAsync(CommonTechnology tech)
    {
        await _repository.AddAsync(tech);
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var tech = await _repository.GetByIdAsync(id);
        if (tech == null) return false;

        // Toggle IsActive status
        tech.IsActive = !(tech.IsActive ?? true);
        await _repository.UpdateAsync(tech);
        
        return true;
    }
}

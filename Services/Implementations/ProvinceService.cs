//KienHM-07/07/2026
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class ProvinceService : IProvinceService
{
    private readonly IProvinceRepository _provinceRepository;

    public ProvinceService(IProvinceRepository provinceRepository)
    {
        _provinceRepository = provinceRepository;
    }

    public async Task<List<Province>> GetAllProvincesAsync()
    {
        return await _provinceRepository.GetAllAsync();
    }

    public async Task<Province?> GetProvinceByIdAsync(int id)
    {
        return await _provinceRepository.GetByIdAsync(id);
    }

    public async Task AddProvinceAsync(Province province)
    {
        await _provinceRepository.AddAsync(province);
    }

    public async Task UpdateProvinceAsync(Province province)
    {
        await _provinceRepository.UpdateAsync(province);
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var province = await _provinceRepository.GetByIdAsync(id);
        if (province == null) return false;

        province.IsActive = !province.IsActive;
        await _provinceRepository.UpdateAsync(province);
        return true;
    }
}

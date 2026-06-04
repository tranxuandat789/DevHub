//AnhPT-02/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICommonTechnologyRepository
{
    Task<List<CommonTechnology>> GetByIdsAsync(List<int> ids);
    Task<List<CommonTechnology>> GetAllActiveAsync();
    Task<CommonTechnology?> GetByIdAsync(int techId);
    Task<List<CommonTechnology>> GetAllAsync();
    Task UpdateAsync(CommonTechnology tech);
    Task AddAsync(CommonTechnology tech);
}

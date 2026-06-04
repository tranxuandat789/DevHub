//AnhPT-02/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICommonJobPositionRepository
{
    Task<CommonJobPosition?> GetByIdAsync(int positionId);
    Task<List<CommonJobPosition>> GetAllActiveAsync();
    Task<List<CommonJobPosition>> GetAllAsync();
    Task UpdateAsync(CommonJobPosition position);
}

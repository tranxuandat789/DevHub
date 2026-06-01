using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICommonJobPositionRepository
{
    Task<CommonJobPosition?> GetByIdAsync(int positionId);
    Task<List<CommonJobPosition>> GetAllActiveAsync();
}

using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICommonTechnologyRepository
{
    Task<List<CommonTechnology>> GetByIdsAsync(List<int> ids);
    Task<List<CommonTechnology>> GetAllActiveAsync();
}

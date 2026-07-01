using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IProvinceRepository
{
    /// All provinces, ordered by name (for dropdowns).
    Task<List<Province>> GetAllAsync();

    /// Load the province entities for the given ids (for assigning to a job post).
    Task<List<Province>> GetByIdsAsync(IEnumerable<int> ids);
}

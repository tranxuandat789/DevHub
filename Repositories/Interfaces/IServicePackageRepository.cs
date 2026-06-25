//AnhPT-02/06/2026
using DevHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces;

public interface IServicePackageRepository
{
    Task<ServicePackage?> GetByIdAsync(int serviceId);
    Task<(List<ServicePackage> Items, int TotalCount)> GetAllPackagesAsync(string searchTerm, string statusFilter, string sortOrder, int page, int pageSize);
    Task<ServicePackage> CreatePackageAsync(ServicePackage package);
    Task<ServicePackage> UpdatePackageAsync(ServicePackage package);
    Task<bool> ToggleStatusAsync(int id, bool activate);
}

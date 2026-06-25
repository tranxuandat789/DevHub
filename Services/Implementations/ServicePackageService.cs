using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Services.Implementations;

public class ServicePackageService : IServicePackageService
{
    private readonly IServicePackageRepository _repository;

    public ServicePackageService(IServicePackageRepository repository)
    {
        _repository = repository;
    }

    public Task<ServicePackage?> GetByIdAsync(int serviceId)
    {
        return _repository.GetByIdAsync(serviceId);
    }

    public Task<(List<ServicePackage> Items, int TotalCount)> GetAllPackagesAsync(string searchTerm, string statusFilter, string sortOrder, int page, int pageSize)
    {
        return _repository.GetAllPackagesAsync(searchTerm, statusFilter, sortOrder, page, pageSize);
    }

    public Task<ServicePackage> CreatePackageAsync(ServicePackage package)
    {
        return _repository.CreatePackageAsync(package);
    }

    public Task<ServicePackage> UpdatePackageAsync(ServicePackage package)
    {
        return _repository.UpdatePackageAsync(package);
    }

    public Task<bool> ToggleStatusAsync(int id, bool activate)
    {
        return _repository.ToggleStatusAsync(id, activate);
    }
}

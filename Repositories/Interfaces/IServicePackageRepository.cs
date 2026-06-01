using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IServicePackageRepository
{
    Task<ServicePackage?> GetByIdAsync(int serviceId);
}

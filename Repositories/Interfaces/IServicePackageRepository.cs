//AnhPT-02/06/2026
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IServicePackageRepository
{
    Task<ServicePackage?> GetByIdAsync(int serviceId);
}

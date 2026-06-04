using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class ServicePackageRepository : IServicePackageRepository
{
    private readonly ItrecruitmentDbContext _context;
    public ServicePackageRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<ServicePackage?> GetByIdAsync(int serviceId)
    {
        return await _context.ServicePackages.FirstOrDefaultAsync(s => s.ServiceId == serviceId && (s.IsActive == true || s.IsActive == null));
    }

    Task<ServicePackage?> IServicePackageRepository.GetByIdAsync(int serviceId)
    {
        throw new NotImplementedException();
    }
}

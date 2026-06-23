using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;

namespace DevHub.Repositories.Implementations;

public class PackageTransactionRepository : IPackageTransactionRepository
{
    private readonly ItrecruitmentDbContext _context;

    public PackageTransactionRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public IQueryable<PackageTransaction> GetAll()
    {
        return _context.PackageTransactions;
    }
}

using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IPackageTransactionRepository
{
    IQueryable<PackageTransaction> GetAll();
}

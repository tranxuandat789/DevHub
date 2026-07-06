using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICompanyPackageHistoryRepository
{
    Task<CompanyPackageHistory?> GetActivePackageForCompanyAsync(int companyId);
    Task DecrementPostsRemainingAsync(int packageHistoryId);
}

using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICompanyPackageHistoryRepository
{
    Task<CompanyPackageHistory?> GetActivePackageForRecruiterAsync(int recruiterId);
    Task DecrementPostsRemainingAsync(int packageHistoryId);
}

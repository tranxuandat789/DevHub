using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IRecruiterPackageHistoryRepository
{
    Task<RecruiterPackageHistory?> GetActivePackageForRecruiterAsync(int recruiterId);
    Task DecrementPostsRemainingAsync(int packageHistoryId);
}

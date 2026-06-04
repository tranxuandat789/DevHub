using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByEmailAsync(string email);
    Task<UserAccount?> GetByGoogleIdAsync(string googleId);
    Task<UserAccount> AddAsync(UserAccount user);
    Task UpdateLastLoginAsync(int userId);
    Task UpdateGoogleIdAsync(int userId, string googleId);
    Task UpdatePasswordAsync(int userId, string passwordHash);

    // Get user by primary key (for reactivation check)
    Task<UserAccount?> GetByIdAsync(int userId);

    // Set is_active flag (soft delete = false, reactivate = true)
    Task SetActiveStatusAsync(int userId, bool isActive);
}

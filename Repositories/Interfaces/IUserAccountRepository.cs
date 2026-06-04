using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByEmailAsync(string email);
    Task<UserAccount?> GetByGoogleIdAsync(string googleId);
    Task<UserAccount> AddAsync(UserAccount user);
    Task UpdateLastLoginAsync(int userId);
    Task UpdateGoogleIdAsync(int userId, string googleId);
}

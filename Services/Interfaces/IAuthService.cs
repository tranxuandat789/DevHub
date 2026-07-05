using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IAuthService
{
    Task<UserAccount?> FindUserByEmailAsync(string email);
    Task<UserAccount?> FindUserByGoogleIdAsync(string googleId);
    bool VerifyPassword(string password, string hash);
    Task<UserAccount> CreateCandidateGoogleAccountAsync(string email, string googleId, string? fullName, string? avatarUrl);
    Task<UserAccount> CreateRecruiterGoogleAccountAsync(string email, string googleId, string? fullName, string? avatarUrl, string? phone, string? position);
    Task UpdateLastLoginAsync(int userId);
    Task LinkGoogleIdAsync(int userId, string googleId);
    Task SyncCandidateAvatarAsync(int candidateId, string avatarUrl);
    Task SyncRecruiterAvatarAsync(int recruiterId, string avatarUrl);
    Task UpdatePasswordAsync(int userId, string passwordHash);
}

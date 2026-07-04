//AnhPT-02/06/2026
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly ItrecruitmentDbContext _db;

    public UserAccountRepository(ItrecruitmentDbContext db) => _db = db;

    public Task<UserAccount?> GetByEmailAsync(string email)
        => _db.UserAccounts
              .AsNoTracking()
              .Include(u => u.Candidate)
              .Include(u => u.Recruiter).ThenInclude(r => r.Company)
              .Include(u => u.Admin)
              .FirstOrDefaultAsync(u => u.Email == email);

    public Task<UserAccount?> GetByGoogleIdAsync(string googleId)
        => _db.UserAccounts
              .AsNoTracking()
              .Include(u => u.Candidate)
              .Include(u => u.Recruiter).ThenInclude(r => r.Company)
              .Include(u => u.Admin)
              .FirstOrDefaultAsync(u => u.GoogleId == googleId);

    public async Task<UserAccount> AddAsync(UserAccount user)
    {
        _db.UserAccounts.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public Task UpdateLastLoginAsync(int userId)
        => _db.UserAccounts
              .Where(u => u.UserId == userId)
              .ExecuteUpdateAsync(s => s
                  .SetProperty(u => u.LastLogin, DateTime.UtcNow)
                  .SetProperty(u => u.LastUpdated, DateTime.UtcNow));

    public Task UpdateGoogleIdAsync(int userId, string googleId)
        => _db.UserAccounts
              .Where(u => u.UserId == userId)
              .ExecuteUpdateAsync(s => s
                  .SetProperty(u => u.GoogleId, googleId)
                  .SetProperty(u => u.LastUpdated, DateTime.UtcNow));

    public Task UpdatePasswordAsync(int userId, string passwordHash)
        => _db.UserAccounts
              .Where(u => u.UserId == userId)
              .ExecuteUpdateAsync(s => s
                  .SetProperty(u => u.PasswordHash, passwordHash)
                  .SetProperty(u => u.LastUpdated, DateTime.UtcNow));

    // Get user account by primary key
    public Task<UserAccount?> GetByIdAsync(int userId)
        => _db.UserAccounts
              .AsNoTracking()
              .FirstOrDefaultAsync(u => u.UserId == userId);

    // Set is_active (false = soft delete, true = reactivate)
    public Task SetActiveStatusAsync(int userId, bool isActive)
        => _db.UserAccounts
              .Where(u => u.UserId == userId)
              .ExecuteUpdateAsync(s => s
                  .SetProperty(u => u.IsActive, isActive)
                  .SetProperty(u => u.LastUpdated, DateTime.UtcNow));
}

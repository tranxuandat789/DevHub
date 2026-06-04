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
              .Include(u => u.Candidate)
              .Include(u => u.Recruiter)
              .Include(u => u.Admin)
              .FirstOrDefaultAsync(u => u.Email == email);

    public Task<UserAccount?> GetByGoogleIdAsync(string googleId)
        => _db.UserAccounts
              .Include(u => u.Candidate)
              .Include(u => u.Recruiter)
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
}

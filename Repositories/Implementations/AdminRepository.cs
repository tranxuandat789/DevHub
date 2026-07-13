// 04/06/2026-DatTX
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class AdminRepository : IAdminRepository
{
    private readonly ItrecruitmentDbContext _db;

    public AdminRepository(ItrecruitmentDbContext db) => _db = db;

    // Get paged moderator list; filter by search (username/full_name) and active status
    public async Task<(List<Admin> Items, int TotalCount)> GetModeratorListAsync(
        string? search, string? filter, int page, int pageSize)
    {
        var query = _db.Admins
            .Include(a => a.AdminNavigation)
            .Where(a => a.Role == "MODERATOR");

        // Apply search: match username OR full_name
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a =>
                a.Username.ToLower().Contains(term) ||
                (a.FullName != null && a.FullName.ToLower().Contains(term)));
        }

        // Apply active/inactive filter
        if (filter == "active")
            query = query.Where(a => a.AdminNavigation.IsActive == true);
        else if (filter == "inactive")
            query = query.Where(a => a.AdminNavigation.IsActive == false);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.AdminNavigation.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    // Get single moderator with UserAccount navigation for edit page
    public Task<Admin?> GetModeratorByIdAsync(int adminId)
        => _db.Admins
              .Include(a => a.AdminNavigation)
              .FirstOrDefaultAsync(a => a.AdminId == adminId && a.Role == "MODERATOR");

    // Add new admin record (UserAccount must already exist)
    public async Task AddAsync(Admin admin)
    {
        _db.Admins.Add(admin);
        await _db.SaveChangesAsync();
    }

    // Update only FullName and Username (email/password are not changed here)
    public Task UpdateModeratorInfoAsync(int adminId, string fullName, string username)
        => _db.Admins
              .Where(a => a.AdminId == adminId)
              .ExecuteUpdateAsync(s => s
                  .SetProperty(a => a.FullName, fullName)
                  .SetProperty(a => a.Username, username));

    // Return true if another admin already uses this username
    public Task<bool> IsUsernameExistsAsync(string username, int excludeAdminId)
        => _db.Admins
              .AnyAsync(a => a.Username == username && a.AdminId != excludeAdminId);

    // Get all admins with role MODERATOR
    public Task<List<Admin>> GetAllModeratorsAsync()
        => _db.Admins
              .Where(a => a.Role == "MODERATOR" && a.AdminNavigation.IsActive == true)
              .ToListAsync();
}

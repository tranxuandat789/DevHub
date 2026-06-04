// 04/06/2026-DatTX
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IAdminRepository
{
    // Get paged list of moderators with optional search and active/inactive filter
    Task<(List<Admin> Items, int TotalCount)> GetModeratorListAsync(string? search, string? filter, int page, int pageSize);

    // Get single moderator by adminId (includes UserAccount navigation)
    Task<Admin?> GetModeratorByIdAsync(int adminId);

    // Add new admin/moderator record to DB
    Task AddAsync(Admin admin);

    // Update only FullName and Username of a moderator
    Task UpdateModeratorInfoAsync(int adminId, string fullName, string username);

    // Check if username already exists (exclude current admin to allow self-update)
    Task<bool> IsUsernameExistsAsync(string username, int excludeAdminId);
}

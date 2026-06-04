// 04/06/2026-DatTX
using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IAdminService
{
    // Get paged moderator list with search and filter
    Task<(List<Admin> Items, int TotalCount)> GetModeratorListAsync(string? search, string? filter, int page, int pageSize);

    // Get single moderator for edit form
    Task<Admin?> GetModeratorByIdAsync(int adminId);

    // Create moderator; reactivates existing inactive account with same email
    Task<(bool Success, string Message)> CreateModeratorAsync(string email, string password, string username, string fullName);

    // Update username and full name only
    Task<(bool Success, string Message)> UpdateModeratorAsync(int adminId, string fullName, string username);

    // Soft-delete: set is_active = false
    Task<bool> DeactivateModeratorAsync(int adminId);
    Task<bool> ReactivateModeratorAsync(int adminId);
}

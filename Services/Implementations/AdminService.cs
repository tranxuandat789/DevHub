// 04/06/2026-DatTX
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly IAdminRepository       _adminRepo;
    private readonly IUserAccountRepository _userRepo;

    public AdminService(IAdminRepository adminRepo, IUserAccountRepository userRepo)
    {
        _adminRepo = adminRepo;
        _userRepo  = userRepo;
    }

    // Delegate list query to repository
    public Task<(List<Admin> Items, int TotalCount)> GetModeratorListAsync(
        string? search, string? filter, int page, int pageSize)
        => _adminRepo.GetModeratorListAsync(search, filter, page, pageSize);

    // Delegate single record fetch to repository
    public Task<Admin?> GetModeratorByIdAsync(int adminId)
        => _adminRepo.GetModeratorByIdAsync(adminId);

    // Create or reactivate a moderator account
    public async Task<(bool Success, string Message, int AdminId)> CreateModeratorAsync(
        string email, string password, string username, string fullName)
    {
        var existingUser = await _userRepo.GetByEmailAsync(email);

        if (existingUser != null)
        {
            // If account exists but is inactive, reactivate it (do not create new)
            if (existingUser.IsActive == false)
            {
                await _userRepo.SetActiveStatusAsync(existingUser.UserId, true);
                return (true, "Tài khoản đã được kích hoạt lại thành công.", existingUser.UserId);
            }

            // Active account with same email already exists
            return (false, "Email này đã được sử dụng bởi một tài khoản đang hoạt động.", 0);
        }

        // Check username uniqueness before inserting
        var isUsernameTaken = await _adminRepo.IsUsernameExistsAsync(username, excludeAdminId: 0);
        if (isUsernameTaken)
            return (false, "Username này đã được sử dụng bởi một tài khoản khác.", 0);

        // Hash password before saving (same as AuthController line 614)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Create user_account row first to get the generated UserId
        var user = await _userRepo.AddAsync(new UserAccount
        {
            Email        = email,
            PasswordHash = passwordHash,
            UserType     = "MODERATOR",
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow,
            LastUpdated  = DateTime.UtcNow
        });

        // Create admin row linked to the new user
        await _adminRepo.AddAsync(new Admin
        {
            AdminId  = user.UserId,
            Username = username,
            FullName = fullName,
            Role     = "MODERATOR"
        });

        return (true, "Tạo tài khoản moderator thành công.", user.UserId);
    }

    // Update moderator profile (username and full name only)
    public async Task<(bool Success, string Message)> UpdateModeratorAsync(
        int adminId, string fullName, string username)
    {
        // Check username uniqueness (exclude self)
        var isTaken = await _adminRepo.IsUsernameExistsAsync(username, adminId);
        if (isTaken)
            return (false, "Username này đã được sử dụng bởi người dùng khác.");

        await _adminRepo.UpdateModeratorInfoAsync(adminId, fullName, username);
        return (true, "Cập nhật thông tin moderator thành công.");
    }

    // Soft delete: set is_active = false in user_account
    public async Task<bool> DeactivateModeratorAsync(int adminId)
    {
        var mod = await _adminRepo.GetModeratorByIdAsync(adminId);
        if (mod == null) return false;

        await _userRepo.SetActiveStatusAsync(adminId, false);
        return true;
    }

    public async Task<bool> ReactivateModeratorAsync(int adminId)
    {
        var mod = await _adminRepo.GetModeratorByIdAsync(adminId);
        if (mod == null) return false;

        await _userRepo.SetActiveStatusAsync(adminId, true);
        return true;
    }
}

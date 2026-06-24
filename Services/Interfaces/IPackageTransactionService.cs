using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IPackageTransactionService
{
    Task<AdminDashboardViewModel> GetAdminDashboardDataAsync(int month, int year);
}

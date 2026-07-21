//AnhPT-07/07/2026
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.Implementations;

public class PackageTransactionService : IPackageTransactionService
{
    private readonly IPackageTransactionRepository _repository;

    public PackageTransactionService(IPackageTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync(int month, int year)
    {
        var query = _repository.GetAll()
            .Include(x => x.Company)
            .Include(x => x.Service)
            .Where(x => x.Status == "Success" || x.Status == "Completed"); // Assuming successful statuses

        // Calculate Quarter based on the given month
        int quarter = (month - 1) / 3 + 1;
        int quarterStartMonth = (quarter - 1) * 3 + 1;
        int quarterEndMonth = quarterStartMonth + 2;

        var allSuccess = await query.ToListAsync();

        var monthTransactions = allSuccess.Where(x => x.TransactionDate?.Month == month && x.TransactionDate?.Year == year).ToList();
        var quarterTransactions = allSuccess.Where(x => x.TransactionDate?.Month >= quarterStartMonth && x.TransactionDate?.Month <= quarterEndMonth && x.TransactionDate?.Year == year).ToList();

        var viewModel = new AdminDashboardViewModel
        {
            SelectedMonth = month,
            SelectedYear = year,
            TotalRevenueMonth = monthTransactions.Sum(x => x.FinalAmount),
            TotalPackagesMonth = monthTransactions.Count,
            TotalRevenueQuarter = quarterTransactions.Sum(x => x.FinalAmount),
            RecentTransactions = allSuccess
                .OrderByDescending(x => x.TransactionDate)
                .Take(5)
                .Select(x => new RecentTransactionDto
                {
                    CompanyName = x.Company?.CompanyName ?? "Unknown",
                    PackageName = x.Service?.PackageName ?? "Unknown Package",
                    Amount = x.FinalAmount,
                    TransactionDate = x.TransactionDate ?? DateTime.MinValue
                }).ToList()
        };

        // Six month revenue calculation
        var sixMonthRevenues = new List<decimal>();
        for (int i = 1; i <= 6; i++)
        {
            var rev = allSuccess.Where(x => x.TransactionDate?.Month == i && x.TransactionDate?.Year == year).Sum(x => x.FinalAmount);
            sixMonthRevenues.Add(rev);
        }
        viewModel.SixMonthRevenue = sixMonthRevenues;

        return viewModel;
    }
}


//KienHM-22/6/2026

using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers.Admin
{
    [Route("AdminDashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IPackageTransactionService _packageTransactionService;
        private readonly ItrecruitmentDbContext _context;

        public AdminDashboardController(IPackageTransactionService packageTransactionService, ItrecruitmentDbContext context)
        {
            _packageTransactionService = packageTransactionService;
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var viewModel = await _packageTransactionService.GetAdminDashboardDataAsync(selectedMonth, selectedYear);

            // Fetch New Statistics
            viewModel.TotalRecruiters = await _context.Recruiters.CountAsync();
            viewModel.TotalCandidates = await _context.Candidates.CountAsync();
            
            // Note: If you don't have a specific table for Moderators, or if they are in UserAccounts
            // Assuming Moderators are UserAccounts with UserType == "Moderator"
            // BR-MOD-02: Admin có quyền quản lý, theo dõi số lượng tài khoản mang role MODERATOR.
            viewModel.TotalModerators = await _context.UserAccounts.CountAsync(u => u.UserType == "Moderator");

            viewModel.ActiveJobPosts = await _context.JobPosts.CountAsync(j => j.Status == "APPROVED");
            viewModel.TotalAppliedCVs = await _context.Applications.CountAsync();

            // 2. Package Distribution
            var packageTxns = await _context.PackageTransactions
                .Where(t => t.Status.ToUpper() == "COMPLETED" || t.Status.ToUpper() == "SUCCESS")
                .Include(t => t.Service)
                .Include(t => t.Company)
                .ToListAsync();

            // 1. Top Recruiters
            // Safest way: Calculate dynamically from actual successful transactions
            viewModel.TopRecruiters = packageTxns
                .Where(t => t.Company != null)
                .GroupBy(t => new { t.CompanyId, t.Company.CompanyName })
                .Select(g => new TopRecruiterDto
                {
                    RecruiterId = g.Key.CompanyId,
                    CompanyName = g.Key.CompanyName ?? "Unknown",
                    TotalSpent = g.Sum(x => x.FinalAmount)
                })
                .OrderByDescending(r => r.TotalSpent)
                .Take(5)
                .ToList();

            viewModel.PackageDistribution = packageTxns
                .GroupBy(t => t.Service?.PackageName ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());

            return View(viewModel);
        }

        [HttpGet("ExportCsv")]
        public async Task<IActionResult> ExportTransactionsCsv()
        {
            var transactions = await _context.PackageTransactions
                .Include(t => t.Company)
                .Include(t => t.Service)
                .Where(t => t.Status == "Completed" || t.Status == "Success")
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Transaction ID,Company,Package,Amount (VND),Date,Payment Method,Status");

            foreach (var txn in transactions)
            {
                var companyName = txn.Company?.CompanyName?.Replace(",", " ") ?? "Unknown";
                var packageName = txn.Service?.PackageName?.Replace(",", " ") ?? "Unknown";
                builder.AppendLine($"{txn.TransactionId},{companyName},{packageName},{txn.FinalAmount},{txn.TransactionDate:yyyy-MM-dd HH:mm},{txn.PaymentMethod},{txn.Status}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"Revenue_Report_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}



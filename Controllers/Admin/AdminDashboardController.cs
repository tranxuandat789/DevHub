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
            viewModel.TotalModerators = await _context.UserAccounts.CountAsync(u => u.UserType == "Moderator");

            viewModel.ActiveJobPosts = await _context.JobPosts.CountAsync(j => j.Status == "Active");
            viewModel.TotalAppliedCVs = await _context.Applications.CountAsync();

            viewModel.TopRecruiters = await _context.Recruiters
                .OrderByDescending(r => r.TotalSpent)
                .Take(5)
                .Select(r => new TopRecruiterDto
                {
                    RecruiterId = r.RecruiterId,
                    CompanyName = r.CompanyName,
                    TotalSpent = r.TotalSpent ?? 0
                }).ToListAsync();

            var packageDistribution = await _context.PackageTransactions
                .Where(t => t.Status == "Completed" || t.Status == "Success")
                .Include(t => t.Service)
                .GroupBy(t => t.Service.PackageName)
                .Select(g => new { PackageName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PackageName ?? "Unknown", x => x.Count);
            viewModel.PackageDistribution = packageDistribution;

            return View(viewModel);
        }

        [HttpGet("ExportCsv")]
        public async Task<IActionResult> ExportTransactionsCsv()
        {
            var transactions = await _context.PackageTransactions
                .Include(t => t.Recruiter)
                .Include(t => t.Service)
                .Where(t => t.Status == "Completed" || t.Status == "Success")
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Transaction ID,Company,Package,Amount (VND),Date,Payment Method,Status");

            foreach (var txn in transactions)
            {
                var companyName = txn.Recruiter?.CompanyName?.Replace(",", " ") ?? "Unknown";
                var packageName = txn.Service?.PackageName?.Replace(",", " ") ?? "Unknown";
                builder.AppendLine($"{txn.TransactionId},{companyName},{packageName},{txn.FinalAmount},{txn.TransactionDate:yyyy-MM-dd HH:mm},{txn.PaymentMethod},{txn.Status}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"Revenue_Report_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}

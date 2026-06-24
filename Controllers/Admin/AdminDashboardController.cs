using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("AdminDashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IPackageTransactionService _packageTransactionService;

        public AdminDashboardController(IPackageTransactionService packageTransactionService)
        {
            _packageTransactionService = packageTransactionService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var viewModel = await _packageTransactionService.GetAdminDashboardDataAsync(selectedMonth, selectedYear);

            return View(viewModel);
        }
    }
}

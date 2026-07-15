using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Models;
using DevHub.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/dashboard")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterDashboardController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IRecruiterDashboardService _dashboardService;
        private readonly IInterviewService _interviewService;
        private readonly ILogger<RecruiterDashboardController> _logger;

        public RecruiterDashboardController(
            IAuthService authService,
            IRecruiterDashboardService dashboardService,
            IInterviewService interviewService,
            ILogger<RecruiterDashboardController> logger)
        {
            _authService = authService;
            _dashboardService = dashboardService;
            _interviewService = interviewService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? jobStatus, 
            string? jobQ, 
            string? jobSort, 
            int jobPage = 1)
        {
            // Resolve the logged-in recruiter (auth concern stays in the controller).
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser?.Recruiter == null)
                return NotFound();

            int recruiterId = dbUser.Recruiter.RecruiterId;
            int companyId = dbUser.Recruiter.CompanyId ?? 0;

            await _interviewService.SyncInterviewStatusesAsync();

            try
            {
                var viewModel = await _dashboardService.GetDashboardAsync(companyId, recruiterId, jobStatus, jobQ, jobSort, jobPage);
                return View("~/Views/Recruiter/RecruiterDashboard/Index.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                // Data retrieval failed — render the page with a safe empty model + an error banner
                // instead of throwing an unhandled exception.
                _logger.LogError(ex, "Failed to load recruiter dashboard for recruiter {RecruiterId}", recruiterId);
                ViewBag.LoadError = "Không thể tải dữ liệu bảng điều khiển. Vui lòng thử lại sau.";
                return View("~/Views/Recruiter/RecruiterDashboard/Index.cshtml", new RecruiterDashboard());
            }
        }
    }
}

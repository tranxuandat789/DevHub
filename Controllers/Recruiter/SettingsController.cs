using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DevHub.Services.Interfaces;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/Settings")]
    [Authorize(Roles = "BUSINESS")]
    public class SettingsController : Controller
    {
        private readonly IAuthService _authService;

        public SettingsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string tab = "account")
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = tab.ToLower();

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser == null || dbUser.Recruiter == null)
            {
                return NotFound();
            }

            return View("~/Views/Recruiter/Settings/Index.cshtml", dbUser.Recruiter);
        }
    }
}

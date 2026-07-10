using DevHub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevHub.Controllers.Moderator
{
    [Route("ModeratorDashboard")]
    [Authorize(Roles = "Moderator")]
    public class ModeratorDashboardController : Controller
    {
        private readonly ItrecruitmentDbContext _context;

        public ModeratorDashboardController(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var taskType = User.FindFirstValue("TaskType");
            ViewBag.TaskType = taskType;
            return View("~/Views/Moderator/ModeratorDashboard/Index.cshtml");
        }
    }
}

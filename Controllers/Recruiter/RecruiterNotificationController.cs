using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/notifications")]
    [Authorize(Roles = "BUSINESS")]
    public class RecruiterNotificationController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = "Notifications";
            return View();
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            ViewData["ActiveMenu"] = "Notifications";
            ViewBag.NotificationId = id;
            return View();
        }
    }
}

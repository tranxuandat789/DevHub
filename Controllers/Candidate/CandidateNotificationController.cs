using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/notifications")]
    public class CandidateNotificationController : Controller
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
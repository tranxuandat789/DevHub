using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/notifications")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateNotificationController : Controller
    {
        [HttpGet("")]
        public IActionResult Notifications()
        {
            ViewData["ActiveMenu"] = "Notifications";
            return View("~/Views/Candidate/CandidateNotification/Index.cshtml");
        }

        [HttpGet("details/{id}")]
        public IActionResult NotificationDetails(int id)
        {
            ViewData["ActiveMenu"] = "Notifications";
            ViewBag.NotificationId = id;
            return View("~/Views/Candidate/CandidateNotification/Details.cshtml");
        }
    }
}

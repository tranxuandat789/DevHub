using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class CandidateController : Controller
    {
        public IActionResult Dashboard()
        {
            var model = new DevHub.Models.DashboardViewModel
            {
                AppliedJobsCount = 24,
                SavedJobsCount = 15,
                InterviewsCount = 3
            };
            return View("~/Views/Candidate/CandidateDashboard/Index.cshtml", model);
        }

        public IActionResult Profile()
        {
            return View("~/Views/Candidate/CandidateProfile/Index.cshtml");
        }

        [HttpPost]
        public IActionResult UploadAvatar(IFormFile avatar)
        {
            if (avatar != null && avatar.Length > 0)
            {
                return Json(new { success = true, message = "Avatar uploaded successfully" });
            }
            return Json(new { success = false, message = "No file selected" });
        }

        public IActionResult AppliedJobs()
        {
            return View("~/Views/Candidate/CandidateApplication/AppliedJobs.cshtml");
        }

        public IActionResult SavedJobs()
        {
            return View("~/Views/Candidate/Bookmark/Index.cshtml");
        }

        public IActionResult Interviews()
        {
            return View("~/Views/Candidate/CandidateInterview/Index.cshtml");
        }

        public IActionResult Notifications()
        {
            return View("~/Views/Candidate/CandidateNotification/Index.cshtml");
        }

        public IActionResult NotificationDetails(int id)
        {
            ViewBag.NotificationId = id;
            return View("~/Views/Candidate/CandidateNotification/Details.cshtml");
        }

        public IActionResult ChangePassword()
        {
            return View("~/Views/Candidate/CandidateProfile/ChangePassword.cshtml");
        }
    }
}

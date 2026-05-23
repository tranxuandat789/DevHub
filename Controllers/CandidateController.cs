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
            return View(model);
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadAvatar(IFormFile avatar)
        {
            if (avatar != null && avatar.Length > 0)
            {
                // In a real application, you would save the file to a cloud storage or wwwroot
                // For now, we'll just simulate a successful upload and return the image URL
                // e.g. var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", avatar.FileName);
                return Json(new { success = true, message = "Avatar uploaded successfully" });
            }
            return Json(new { success = false, message = "No file selected" });
        }

        public IActionResult AppliedJobs()
        {
            return View();
        }

        public IActionResult SavedJobs()
        {
            return View();
        }

        public IActionResult Interviews()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult NotificationDetails(int id)
        {
            ViewBag.NotificationId = id;
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}

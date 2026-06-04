using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/dashboard")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateDashboardController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            var model = new DevHub.Models.DashboardViewModel
            {
                AppliedJobsCount = 24,
                SavedJobsCount = 15,
                InterviewsCount = 3
            };
            return View("~/Views/Candidate/CandidateDashboard/Index.cshtml", model);
        }
    }
}

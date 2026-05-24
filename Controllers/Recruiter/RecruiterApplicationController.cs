using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    // [Authorize(Roles = "Recruiter")]
    public class RecruiterApplicationController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Recruiter/RecruiterApplication/Index.cshtml");
        }

        [HttpGet("Details")]
        public IActionResult Details()
        {
            return View("~/Views/Recruiter/RecruiterApplication/Details.cshtml");
        }
    }
}

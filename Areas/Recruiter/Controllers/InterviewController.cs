using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class InterviewController : Controller
    {
        public IActionResult Schedule() => View();
    }
}

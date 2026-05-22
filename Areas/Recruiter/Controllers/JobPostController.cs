using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class JobPostController : Controller
    {
        public IActionResult Create() => View();
        public IActionResult Index() => View();
        public IActionResult Edit() => View();
        public IActionResult Applicants() => View();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class RecruiterProfileController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult CompanyProfile() => View();
    }
}

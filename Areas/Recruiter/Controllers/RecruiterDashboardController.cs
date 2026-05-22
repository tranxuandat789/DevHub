using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class RecruiterDashboardController : Controller
    {
        public IActionResult Index() => View();
    }
}

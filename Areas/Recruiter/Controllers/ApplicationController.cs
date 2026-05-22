using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class ApplicationController : Controller
    {
        public IActionResult CheckProfile() => View();
    }
}

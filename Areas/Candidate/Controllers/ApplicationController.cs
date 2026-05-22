using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class ApplicationController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Applying() => View();
    }
}

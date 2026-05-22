using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class CandidateDashboardController : Controller
    {
        public IActionResult Index() => View();
    }
}

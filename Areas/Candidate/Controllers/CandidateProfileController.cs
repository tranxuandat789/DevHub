using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class CandidateProfileController : Controller
    {
        public IActionResult Index() => View();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class InterviewCandidateController : Controller
    {
        public IActionResult Index() => View();
    }
}

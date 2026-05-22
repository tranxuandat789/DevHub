using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class ReviewController : Controller
    {
        public IActionResult Create() => View();
    }
}

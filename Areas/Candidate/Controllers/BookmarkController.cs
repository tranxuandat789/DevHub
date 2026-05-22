using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class BookmarkController : Controller
    {
        public IActionResult Index() => View();
    }
}

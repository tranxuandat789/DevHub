using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class ReviewController : Controller
    {
        public IActionResult Index() => View();
    }
}

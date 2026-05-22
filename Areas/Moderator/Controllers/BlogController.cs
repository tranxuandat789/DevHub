using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class BlogController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
    }
}

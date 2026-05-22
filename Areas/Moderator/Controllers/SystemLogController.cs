using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class SystemLogController : Controller
    {
        public IActionResult Index() => View();
    }
}

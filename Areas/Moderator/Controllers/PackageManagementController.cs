using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class PackageManagementController : Controller
    {
        public IActionResult Index() => View();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class UserManagementController : Controller
    {
        public IActionResult Index() => View();
    }
}

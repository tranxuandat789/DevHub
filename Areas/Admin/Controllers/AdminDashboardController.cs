using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminDashboardController : Controller
    {
        public IActionResult Index() => View();
    }
}

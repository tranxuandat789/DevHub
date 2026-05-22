using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class NotificationController : Controller
    {
        public IActionResult Index() => View();
    }
}

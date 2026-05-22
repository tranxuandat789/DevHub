using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class JobApprovalController : Controller
    {
        public IActionResult Index() => View();
    }
}

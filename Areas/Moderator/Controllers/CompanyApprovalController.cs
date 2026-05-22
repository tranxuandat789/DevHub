using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class CompanyApprovalController : Controller
    {
        public IActionResult Index() => View();
    }
}

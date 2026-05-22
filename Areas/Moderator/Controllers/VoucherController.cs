using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class VoucherController : Controller
    {
        public IActionResult Index() => View();
    }
}

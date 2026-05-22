using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    public class MasterDataController : Controller
    {
        public IActionResult JobPosition() => View();
        public IActionResult Technology() => View();
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/Settings")]
    // [Authorize(Roles = "Recruiter")]
    public class SettingsController : Controller
    {
        [HttpGet("")]
        public IActionResult Index(string tab = "account")
        {
            ViewData["ActiveMenu"] = "Settings";
            ViewBag.ActiveTab = tab.ToLower();
            return View("~/Views/Recruiter/Settings/Index.cshtml");
        }
    }
}

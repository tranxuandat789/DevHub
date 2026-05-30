using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/packages")]
    [Authorize(Roles = "Moderator")]
    public class PackageManagementController : Controller
    {
        [HttpGet("")]
        [HttpGet("/ModeratorPackage")]
        public IActionResult Index()
        {
            return View("~/Views/Moderator/PackageManagement/Index.cshtml");
        }
    }
}

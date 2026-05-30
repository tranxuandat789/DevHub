using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/users")]
    [Authorize(Roles = "Moderator")]
    public class ModeratorUserManagementController : Controller
    {
        [HttpGet("")]
        [HttpGet("/ModeratorUser")]
        public IActionResult Index()
        {
            return View("~/Views/Moderator/ModeratorUserManagement/Index.cshtml");
        }
    }
}

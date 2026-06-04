using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/logs")]
    [Authorize(Roles = "Moderator")]
    public class SystemLogController : Controller
    {
        [HttpGet("")]
        [HttpGet("/SystemLog")]
        public IActionResult Index()
        {
            return View("~/Views/Moderator/SystemLog/Index.cshtml");
        }
    }
}

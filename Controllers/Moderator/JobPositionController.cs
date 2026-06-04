using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/job-positions")]
    [Authorize(Roles = "Moderator")]
    public class JobPositionController : Controller
    {
          [HttpGet("")]
        [HttpGet("/ModeratorPosition")]
        public IActionResult Index()
        {
            return View("~/Views/Moderator/JobPosition/Index.cshtml");
        }
    }
}

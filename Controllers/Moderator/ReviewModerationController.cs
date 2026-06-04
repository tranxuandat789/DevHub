using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/reviews")]
    [Authorize(Roles = "Moderator")]
    public class ReviewModerationController : Controller
    {
        [HttpGet("")]

        public IActionResult Index()
        {
            return View("~/Views/Moderator/ReviewApproval/Index.cshtml");
        }
    }
}

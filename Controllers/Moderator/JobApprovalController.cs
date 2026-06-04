using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/job-approvals")]
    [Authorize(Roles = "Moderator")]
    public class JobApprovalController : Controller
    {
        [HttpGet("")]
        [HttpGet("/JobApproval")]
        public IActionResult Index()
        {
            return View("~/Views/Moderator/JobApproval/Index.cshtml");
        }
    }
}

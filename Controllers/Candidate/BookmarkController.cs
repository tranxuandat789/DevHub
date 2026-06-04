using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/bookmarks")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class BookmarkController : Controller
    {
        [HttpGet("")]
        public IActionResult SavedJobs()
        {
            return View("~/Views/Candidate/Bookmark/Index.cshtml");
        }
    }
}

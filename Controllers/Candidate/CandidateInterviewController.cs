using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/interviews")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateInterviewController : Controller
    {
        [HttpGet("")]
        public IActionResult Interviews()
        {
            return View("~/Views/Candidate/CandidateInterview/Index.cshtml");
        }
    }
}

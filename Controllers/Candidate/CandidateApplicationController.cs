using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/applications")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateApplicationController : Controller
    {
        [HttpGet("")]
        public IActionResult AppliedJobs()
        {
            return View("~/Views/Candidate/CandidateApplication/AppliedJobs.cshtml");
        }
    }
}

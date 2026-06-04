using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/interviews")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateInterviewController : Controller
    {
    }
}
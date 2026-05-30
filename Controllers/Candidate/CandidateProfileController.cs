using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/profile")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateProfileController : Controller
    {
    }
}

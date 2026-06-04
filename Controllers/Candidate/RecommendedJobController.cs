using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/jobs/recommended")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class RecommendedJobController : Controller
    {
    }
}
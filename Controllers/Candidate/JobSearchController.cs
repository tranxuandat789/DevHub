using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/jobs")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class JobSearchController : Controller
    {
    }
}

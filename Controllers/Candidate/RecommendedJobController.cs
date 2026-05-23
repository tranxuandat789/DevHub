using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/jobs/recommended")]
    [Authorize(Roles = "Candidate")]
    public class RecommendedJobController : Controller
    {
    }
}

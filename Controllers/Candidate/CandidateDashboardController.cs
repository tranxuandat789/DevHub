using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/dashboard")]
    [Authorize(Roles = "Candidate")]
    public class CandidateDashboardController : Controller
    {
    }
}

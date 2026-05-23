using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/notifications")]
    [Authorize(Roles = "Candidate")]
    public class CandidateNotificationController : Controller
    {
    }
}

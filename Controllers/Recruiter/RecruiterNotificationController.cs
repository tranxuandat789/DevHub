using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/notifications")]
    [Authorize(Roles = "Recruiter")]
    public class RecruiterNotificationController : Controller
    {
    }
}

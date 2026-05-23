using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/job-positions")]
    [Authorize(Roles = "Moderator")]
    public class JobPositionController : Controller
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/job-approvals")]
    [Authorize(Roles = "Moderator")]
    public class JobApprovalController : Controller
    {
    }
}

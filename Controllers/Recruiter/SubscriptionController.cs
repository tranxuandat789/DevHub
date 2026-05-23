using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/subscription")]
    [Authorize(Roles = "Recruiter")]
    public class SubscriptionController : Controller
    {
    }
}

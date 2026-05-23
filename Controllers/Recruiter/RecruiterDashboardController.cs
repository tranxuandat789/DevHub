using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/dashboard")]
    [Authorize(Roles = "Recruiter")]
    public class RecruiterDashboardController : Controller
    {
    }
}

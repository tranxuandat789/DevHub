using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/applications")]
    [Authorize(Roles = "Recruiter")]
    public class RecruiterApplicationController : Controller
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/jobs")]
    [Authorize(Roles = "Recruiter")]
    public class JobPostController : Controller
    {
    }
}

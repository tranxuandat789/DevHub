using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/interviews")]
    [Authorize(Roles = "Recruiter")]
    public class RecruiterInterviewController : Controller
    {
    }
}

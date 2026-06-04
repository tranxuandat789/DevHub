using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/profile")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterProfileController : Controller
    {
    }
}

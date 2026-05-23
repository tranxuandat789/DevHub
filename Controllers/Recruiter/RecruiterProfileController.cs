using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/profile")]
    [Authorize(Roles = "Recruiter")]
    public class RecruiterProfileController : Controller
    {
    }
}

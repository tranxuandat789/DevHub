using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/techstack")]
    [Authorize(Roles = "Moderator")]
    public class TechStackController : Controller
    {
    }
}

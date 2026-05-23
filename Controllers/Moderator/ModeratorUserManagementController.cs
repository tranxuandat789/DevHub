using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/users")]
    [Authorize(Roles = "Moderator")]
    public class ModeratorUserManagementController : Controller
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/moderators")]
    [Authorize(Roles = "Admin")]
    public class ModeratorManagementController : Controller
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserManagementController : Controller
    {
    }
}

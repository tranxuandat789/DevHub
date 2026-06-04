using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserManagementController : Controller
    {
        [HttpGet("")]
        [HttpGet("/AdminUser")]
        public IActionResult Index()
        {
            return View("~/Views/Admin/AdminUserManagement/Index.cshtml");
        }
    }
}

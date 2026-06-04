using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/moderators")]
    [Authorize(Roles = "Admin")]
    public class ModeratorManagementController : Controller
    {
        [HttpGet("")]
        [HttpGet("/AdminModerator")]
        public IActionResult Index()
        {
            return View("~/Views/Admin/AdminModerator/Index.cshtml");
        }

        [HttpGet("create")]
        [HttpGet("/AdminModerator/Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/AdminModerator/Create.cshtml");
        }
    }
}

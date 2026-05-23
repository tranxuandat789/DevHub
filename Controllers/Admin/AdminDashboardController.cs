using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
    }
}

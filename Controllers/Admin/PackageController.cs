using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/packages")]
    [Authorize(Roles = "Admin")]
    public class PackageController : Controller
    {
    }
}

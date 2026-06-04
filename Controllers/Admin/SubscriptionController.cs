using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/subscriptions")]
    [Authorize(Roles = "Admin")]
    public class SubscriptionController : Controller
    {
        [HttpGet("")]
        [HttpGet("/AdminSubscription")]
        public IActionResult Index()
        {
            return View("~/Views/Admin/AdminSubscription/Index.cshtml");
        }
    }
}

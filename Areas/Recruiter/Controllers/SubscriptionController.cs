using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class SubscriptionController : Controller
    {
        public IActionResult Index() => View();
    }
}

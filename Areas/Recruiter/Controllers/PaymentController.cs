using Microsoft.AspNetCore.Mvc;

namespace DevHub.Areas.Recruiter.Controllers
{
    [Area("Recruiter")]
    public class PaymentController : Controller
    {
        public IActionResult Confirmation() => View();
        public IActionResult History() => View();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Authorize(Roles = "BUSINESS")]
    [Route("Recruiter/[controller]")]
    public class RecruiterServiceController : Controller
    {
        [HttpGet]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = "Services";
            return View("~/Views/Recruiter/RecruiterService/Index.cshtml");
        }

        [HttpGet("Checkout")]
        public IActionResult Checkout(string package = "silver")
        {
            ViewData["ActiveMenu"] = "Services";
            ViewBag.Package = package;
            return View("~/Views/Recruiter/RecruiterService/Checkout.cshtml");
        }

        [HttpGet("History")]
        public IActionResult History()
        {
            ViewData["ActiveMenu"] = "Services";
            return View("~/Views/Recruiter/RecruiterService/History.cshtml");
        }
    }
}

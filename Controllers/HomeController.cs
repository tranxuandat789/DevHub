using System.Diagnostics;
using DevHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Employer()
        {
            return View("~/Views/Recruiter/Home/Index.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error404")]
        public IActionResult Error404()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

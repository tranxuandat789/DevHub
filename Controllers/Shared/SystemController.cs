using Microsoft.AspNetCore.Mvc;
using DevHub.Models;
using System.Diagnostics;

namespace DevHub.Controllers.Shared
{
    public class SystemController : Controller
    {
        [Route("Home/Privacy")]
        public IActionResult Privacy()
        {
            return View("~/Views/Home/Privacy.cshtml");
        }

        [Route("Home/Error404")]
        public IActionResult Error404()
        {
            return View("~/Views/Home/Error404.cshtml");
        }

        [Route("Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("~/Views/Home/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

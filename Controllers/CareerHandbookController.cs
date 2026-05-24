using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class CareerHandbookController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Blog/Index.cshtml");
        }

        public IActionResult Details(int id)
        {
            return View("~/Views/Blog/Details.cshtml");
        }
    }
}

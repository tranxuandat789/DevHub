using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class JobsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id = 1)
        {
            return View();
        }
    }
}

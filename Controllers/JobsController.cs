using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class JobsController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Candidate/Job/Index.cshtml");
        }

        public IActionResult Details(int id = 1)
        {
            return View("~/Views/Candidate/Job/Details.cshtml");
        }
    }
}

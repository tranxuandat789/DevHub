using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class CompaniesController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Candidate/Company/Index.cshtml");
        }

        public IActionResult Details(int id = 1)
        {
            return View("~/Views/Candidate/Company/Details.cshtml");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class CompaniesController : Controller
    {
        public IActionResult Details(int id = 1)
        {
            return View();
        }
    }
}

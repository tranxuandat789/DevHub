using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Shared
{
    [Route("companies")]
    public class CompanyController : Controller
    {
        [Route("")]
        [Route("/Companies")]
        public IActionResult Index()
        {
            return View("~/Views/Candidate/Company/Index.cshtml");
        }

        [Route("{id?}")]
        [Route("/Companies/Details/{id?}")]
        public IActionResult Details(int id = 1)
        {
            return View("~/Views/Candidate/Company/Details.cshtml");
        }
    }
}

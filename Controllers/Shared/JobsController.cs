using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Shared
{
    [Route("Jobs")]
    public class JobsController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View("~/Views/Candidate/Job/Index.cshtml");
        }

        [Route("Details/{id?}")]
        public IActionResult Details(int id = 1)
        {
            return View("~/Views/Candidate/Job/Details.cshtml");
        }
    }
}

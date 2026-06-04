using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    public class CandidateHomeController : Controller
    {
        [Route("")]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}

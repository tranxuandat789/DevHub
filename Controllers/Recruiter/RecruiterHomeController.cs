using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    public class RecruiterHomeController : Controller
    {
        [Route("Home/Recruiter")]
        [Route("recruiter/home")]
        public IActionResult Index()
        {
            return View("~/Views/Recruiter/Home/Index.cshtml");
        }
    }
}

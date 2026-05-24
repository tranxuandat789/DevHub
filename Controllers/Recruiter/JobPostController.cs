using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    // [Authorize(Roles = "Employer,Recruiter")]
    public class JobPostController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("Edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}

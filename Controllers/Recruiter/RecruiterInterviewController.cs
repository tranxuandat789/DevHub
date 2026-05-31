using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterInterviewController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Recruiter/RecruiterInterview/Index.cshtml");
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Recruiter/RecruiterInterview/Create.cshtml");
        }
        
        [HttpGet("Edit/{id?}")]
        public IActionResult Edit(int? id)
        {
            return View("~/Views/Recruiter/RecruiterInterview/Edit.cshtml");
        }
        
        [HttpGet("Details/{id?}")]
        public IActionResult Details(int? id)
        {
            return View("~/Views/Recruiter/RecruiterInterview/Details.cshtml");
        }
    }
}

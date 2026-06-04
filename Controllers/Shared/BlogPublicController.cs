using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Shared
{
    [Route("blog")]
    public class BlogPublicController : Controller
    {
        [Route("")]
        [Route("/CareerHandbook")]
        public IActionResult Index()
        {
            return View("~/Views/Blog/Index.cshtml");
        }

        [Route("{id}")]
        [Route("/CareerHandbook/Details/{id?}")]
        public IActionResult Details(int id)
        {
            return View("~/Views/Blog/Details.cshtml");
        }
    }
}

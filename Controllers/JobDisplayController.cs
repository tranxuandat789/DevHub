using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[Route("Job/[action]")]
public class JobDisplayController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Details(int id) => View();
    public IActionResult Recommended() => View();
    public IActionResult CompanyJobs() => View();
}

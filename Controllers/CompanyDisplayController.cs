using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[Route("Company/[action]")]
public class CompanyDisplayController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Details(int id) => View();
}

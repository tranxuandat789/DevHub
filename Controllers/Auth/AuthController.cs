using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Auth;

[Route("Auth/[action]")]
public class AuthController : Controller
{
    public IActionResult Login() => View();
    public IActionResult Register() => View();
    public IActionResult RegisterCandidate() => View();
    public IActionResult RegisterRecruiter() => View();
}

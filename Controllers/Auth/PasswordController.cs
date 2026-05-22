using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Auth;

[Route("Auth/[action]")]
public class PasswordController : Controller
{
    public IActionResult ForgotPassword() => View();
    public IActionResult ResetPassword() => View();
}

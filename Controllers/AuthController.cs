using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult EmployerLogin()
        {
            return View();
        }

        public IActionResult EmployerRegister()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult VerifyOTP()
        {
            return View();
        }
    }
}

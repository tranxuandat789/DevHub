using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/payment")]
    [Authorize(Roles = "RECRUITER")]
    public class PaymentController : Controller
    {
    }
}

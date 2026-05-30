using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/payment")]
    [Authorize(Roles = "BUSINESS")]
    public class PaymentController : Controller
    {
    }
}

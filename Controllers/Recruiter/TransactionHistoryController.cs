using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/transactions")]
    [Authorize(Roles = "RECRUITER")]
    public class TransactionHistoryController : Controller
    {
    }
}

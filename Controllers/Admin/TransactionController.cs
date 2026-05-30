using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("admin/transactions")]
    [Authorize(Roles = "Admin")]
    public class TransactionController : Controller
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/vouchers")]
    [Authorize(Roles = "Moderator")]
    public class VoucherController : Controller
    {
    }
}

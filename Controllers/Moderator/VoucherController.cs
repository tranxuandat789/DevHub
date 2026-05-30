using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/vouchers")]
    [Authorize(Roles = "Moderator")]
    public class VoucherController : Controller
    {
        [HttpGet("")]
        [HttpGet("/ModeratorVoucher")]
        public IActionResult Index()
        {
            return View("~/Views/Moderator/Voucher/Index.cshtml");
        }
    }
}

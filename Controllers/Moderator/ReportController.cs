using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/reports")]
    [Authorize(Roles = "Moderator")]
    public class ReportController : Controller
    {
    }
}

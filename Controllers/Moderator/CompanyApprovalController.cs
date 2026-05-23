using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/company-approvals")]
    [Authorize(Roles = "Moderator")]
    public class CompanyApprovalController : Controller
    {
    }
}

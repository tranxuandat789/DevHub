using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/reviews")]
    [Authorize(Roles = "Candidate")]
    public class ReviewController : Controller
    {
    }
}

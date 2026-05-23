using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/blogs")]
    [Authorize(Roles = "Moderator")]
    public class BlogController : Controller
    {
    }
}

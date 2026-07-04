using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Recruiter;

[Authorize(AuthenticationSchemes = "EmployerCookies")]
[Route("Recruiter/Article")]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var recruiterId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        var articles = await _articleService.GetArticlesForRecruiterAsync(recruiterId);
        return View("~/Views/Recruiter/Article/Index.cshtml", articles);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string content, string thumbnailUrl)
    {
        var recruiterId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        try
        {
            await _articleService.CreateArticleAsync(recruiterId, title, content, thumbnailUrl);
            TempData["SuccessMsg"] = "Đăng bài viết thành công, đang chờ duyệt.";
        }
        catch (System.Exception ex)
        {
            TempData["ErrorMsg"] = ex.Message;
        }
        return RedirectToAction("Index");
    }

    [HttpPost("SubmitReview/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(int id)
    {
        var recruiterId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        try
        {
            await _articleService.SubmitArticleForReviewAsync(recruiterId, id);
            TempData["SuccessMsg"] = "Gửi yêu cầu duyệt lại thành công.";
        }
        catch (System.Exception ex)
        {
            TempData["ErrorMsg"] = ex.Message;
        }
        return RedirectToAction("Index");
    }
}

using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevHub.Controllers.Moderator
{
    [Route("ModeratorArticle")]
    [Authorize(Roles = "Moderator")]
    public class ArticleModerationController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ItrecruitmentDbContext _context;

        public ArticleModerationController(IArticleService articleService, ItrecruitmentDbContext context)
        {
            _articleService = articleService;
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int? companyId, string keyword, int page = 1)
        {
            int pageSize = 10;
            var (articles, totalPages, totalItems) = await _articleService.GetArticlesForModeratorAsync(companyId, keyword, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Keyword = keyword;
            ViewBag.CompanyId = companyId;

            // Load companies for the filter dropdown
            var companies = await _context.Companies
                .OrderBy(c => c.CompanyName)
                .Select(c => new { c.CompanyId, c.CompanyName })
                .ToListAsync();
            ViewBag.Companies = new SelectList(companies, "CompanyId", "CompanyName", companyId);

            return View("~/Views/Moderator/ArticleModeration/Index.cshtml", articles);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null) return NotFound();

            return View("~/Views/Moderator/ArticleModeration/Details.cshtml", article);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id, [FromForm] string reason)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null) return NotFound(new { message = "Không tìm thấy bài viết" });

            var modIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);

            var success = await _articleService.DeleteArticleByModeratorAsync(id, modId, reason);
            if (!success) return BadRequest(new { message = "Xóa bài viết thất bại" });

            var auditLog = new AuditLog
            {
                UserId = modId,
                UserType = "Moderator",
                Action = "Xóa bài viết (Article)",
                EntityType = "Article",
                EntityId = id,
                OldValue = "Tồn tại",
                NewValue = "Đã xóa (Lý do: " + reason + ")",
                CreatedAt = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Moderator;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/articles")]
    [Authorize(Roles = "Moderator")]
    public class ModeratorArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly DevHub.Data.ItrecruitmentDbContext _db;

        public ModeratorArticleController(IArticleService articleService, DevHub.Data.ItrecruitmentDbContext db)
        {
            _articleService = articleService;
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? keyword, string? dateFrom, string? status, string? companyName, int page = 1)
        {
            const int pageSize = 10;
            var (pageItems, totalPages, totalCount) = await _articleService.GetArticlesForModeratorAsync(keyword, dateFrom, status, companyName, page, pageSize);

            page = Math.Min(Math.Max(1, page), Math.Max(1, totalPages));

            var viewModel = new ModeratorArticleViewModel
            {
                Articles = pageItems,
                Keyword = keyword,
                DateFrom = dateFrom,
                Status = status,
                CompanyName = companyName,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                FromItem = totalCount == 0 ? 0 : (page - 1) * pageSize + 1,
                ToItem = Math.Min(page * pageSize, totalCount)
            };

            return View("~/Views/Moderator/Article/Index.cshtml", viewModel);
        }

        [HttpPost("hide/{id}")]
        public async Task<IActionResult> Hide(int id, [FromForm] string reason)
        {
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            try
            {
                await _articleService.HideArticleByModAsync(id, reason);

                var auditLog = new DevHub.Models.AuditLog
                {
                    UserId = moderatorId,
                    UserType = "Moderator",
                    Action = "Ẩn bài báo",
                    EntityType = "Article",
                    EntityId = id,
                    NewValue = "Lý do: " + reason,
                    CreatedAt = DateTime.UtcNow
                };
                _db.AuditLogs.Add(auditLog);
                await _db.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id, [FromForm] string reason)
        {
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            try
            {
                await _articleService.DeleteArticleByModAsync(id, reason);

                var auditLog = new DevHub.Models.AuditLog
                {
                    UserId = moderatorId,
                    UserType = "Moderator",
                    Action = "Xóa bài báo",
                    EntityType = "Article",
                    EntityId = id,
                    NewValue = "Lý do: " + reason,
                    CreatedAt = DateTime.UtcNow
                };
                _db.AuditLogs.Add(auditLog);
                await _db.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

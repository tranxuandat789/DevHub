using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Moderator;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/articles")]
    [Authorize(Roles = "Moderator")]
    public class ModeratorArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly DevHub.Data.ItrecruitmentDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly DevHub.Helpers.EmailHelper _emailHelper;

        public ModeratorArticleController(IArticleService articleService, DevHub.Data.ItrecruitmentDbContext db, INotificationService notificationService, DevHub.Helpers.EmailHelper emailHelper)
        {
            _articleService = articleService;
            _db = db;
            _notificationService = notificationService;
            _emailHelper = emailHelper;
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

                var article = await _db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleId == id);
                if (article != null)
                {
                    var recruiters = await _db.Recruiters
                        .Include(r => r.RecruiterNavigation)
                        .Where(r => r.CompanyId == article.CompanyId)
                        .ToListAsync();

                    foreach (var recruiter in recruiters)
                    {
                        try
                        {
                            await _notificationService.SendNotificationAsync(
                                userId: recruiter.RecruiterId,
                                userType: "RECRUITER",
                                title: "Bài viết bị ẩn",
                                message: $"Bài viết \"{article.Title}\" của bạn đã bị kiểm duyệt viên ẩn. Lý do: {reason}",
                                type: "ARTICLE",
                                severity: "warning",
                                referenceId: id,
                                referenceType: "Article"
                            );

                            if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email) && recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                            {
                                var emailSubject = $"[DevHub] Bài viết bị ẩn: {article.Title}";
                                var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                    "Bài viết bị ẩn",
                                    $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                    $"<p>Rất tiếc, bài viết <strong>\"{article.Title}\"</strong> của bạn đã bị kiểm duyệt viên ẩn khỏi hệ thống.</p>" +
                                    $"<p><strong>Lý do:</strong> {reason}</p>" +
                                    $"<p>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</p>" +
                                    $"<p>Cảm ơn bạn đã đồng hành cùng DevHub!</p>"
                                );
                                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                            }
                        }
                        catch { /* best-effort */ }
                    }
                }

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
                var errMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errMsg += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errMsg += " | Inner2: " + ex.InnerException.InnerException.Message;
                    }
                }
                return BadRequest(errMsg);
            }
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            try
            {
                await _articleService.ApproveArticleByModAsync(id);

                var article = await _db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleId == id);
                if (article != null)
                {
                    var recruiters = await _db.Recruiters
                        .Include(r => r.RecruiterNavigation)
                        .Where(r => r.CompanyId == article.CompanyId)
                        .ToListAsync();

                    foreach (var recruiter in recruiters)
                    {
                        try
                        {
                            await _notificationService.SendNotificationAsync(
                                userId: recruiter.RecruiterId,
                                userType: "RECRUITER",
                                title: "Bài viết được duyệt",
                                message: $"Bài viết \"{article.Title}\" của bạn đã được kiểm duyệt viên duyệt và xuất bản.",
                                type: "ARTICLE",
                                severity: "success",
                                referenceId: id,
                                referenceType: "Article"
                            );

                            if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email) && recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                            {
                                var emailSubject = $"[DevHub] Bài viết được duyệt: {article.Title}";
                                var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                    "Bài viết được duyệt",
                                    $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                    $"<p>Chúc mừng! Bài viết <strong>\"{article.Title}\"</strong> của bạn đã được kiểm duyệt viên duyệt và chính thức xuất bản trên hệ thống.</p>" +
                                    $"<p>Cảm ơn bạn đã đóng góp nội dung chất lượng cho DevHub!</p>"
                                );
                                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                            }
                        }
                        catch { /* best-effort */ }
                    }
                }

                var auditLog = new DevHub.Models.AuditLog
                {
                    UserId = moderatorId,
                    UserType = "Moderator",
                    Action = "Duyệt bài báo",
                    EntityType = "Article",
                    EntityId = id,
                    NewValue = "Status -> PUBLISHED",
                    CreatedAt = DateTime.UtcNow
                };
                _db.AuditLogs.Add(auditLog);
                await _db.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
                return BadRequest(errMsg);
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
                var article = await _db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleId == id);
                
                await _articleService.DeleteArticleByModAsync(id, reason);

                if (article != null)
                {
                    var recruiters = await _db.Recruiters
                        .Include(r => r.RecruiterNavigation)
                        .Where(r => r.CompanyId == article.CompanyId)
                        .ToListAsync();

                    foreach (var recruiter in recruiters)
                    {
                        try
                        {
                            // BR-NTF-01 & BR-NTF-03: Gửi thông báo (In-App) real-time khi Moderator xóa/từ chối bài viết của Recruiter.
                            await _notificationService.SendNotificationAsync(
                                userId: recruiter.RecruiterId,
                                userType: "RECRUITER",
                                title: "Bài viết bị xóa",
                                message: $"Bài viết \"{article.Title}\" của bạn đã bị kiểm duyệt viên xóa khỏi hệ thống. Lý do: {reason}",
                                type: "ARTICLE",
                                severity: "danger",
                                referenceId: id,
                                referenceType: "Article"
                            );

                            if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email) && recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                            {
                                // BR-NTF-03: Kênh thông báo bổ sung qua Email Delivery.
                                var emailSubject = $"[DevHub] Bài viết bị xóa: {article.Title}";
                                var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                    "Bài viết bị xóa",
                                    $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                    $"<p>Rất tiếc, bài viết <strong>\"{article.Title}\"</strong> của bạn đã bị kiểm duyệt viên xóa khỏi hệ thống.</p>" +
                                    $"<p><strong>Lý do:</strong> {reason}</p>" +
                                    $"<p>Vui lòng liên hệ hỗ trợ nếu bạn cần thêm thông tin.</p>" +
                                    $"<p>Cảm ơn bạn đã đồng hành cùng DevHub!</p>"
                                );
                                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                            }
                        }
                        catch { /* best-effort */ }
                    }
                }

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
                var errMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errMsg += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errMsg += " | Inner2: " + ex.InnerException.InnerException.Message;
                    }
                }
                return BadRequest(errMsg);
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var article = await _db.Articles
                .Include(a => a.Company)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ArticleId == id);
                
            if (article == null) return NotFound();
            
            return View("~/Views/Moderator/Article/Detail.cshtml", article);
        }

        [HttpPost("approve-pending/{id}")]
        public async Task<IActionResult> ApprovePending(int id)
        {
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            try
            {
                await _articleService.ApproveArticleByModAsync(id);

                var article = await _db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleId == id);
                if (article != null)
                {
                    var recruiters = await _db.Recruiters
                        .Include(r => r.RecruiterNavigation)
                        .Where(r => r.CompanyId == article.CompanyId)
                        .ToListAsync();

                    foreach (var recruiter in recruiters)
                        {
                            try
                            {
                                await _notificationService.SendNotificationAsync(
                                    userId: recruiter.RecruiterId,
                                    userType: "RECRUITER",
                                    title: "Bài viết được duyệt",
                                    message: $"Bài viết \"{article.Title}\" của bạn đã được kiểm duyệt viên duyệt và hiện đang hiển thị.",
                                    type: "ARTICLE",
                                    severity: "success",
                                    referenceId: id,
                                    referenceType: "Article"
                                );

                                if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email) && recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                                {
                                    var emailSubject = $"[DevHub] Bài viết được duyệt: {article.Title}";
                                    var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                        "Bài viết được duyệt",
                                        $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                        $"<p>Chúc mừng! Bài viết <strong>\"{article.Title}\"</strong> của bạn đã được kiểm duyệt viên duyệt và chính thức hiển thị trên hệ thống.</p>" +
                                        $"<p>Cảm ơn bạn đã đóng góp nội dung chất lượng cho DevHub!</p>"
                                    );
                                    await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                                }
                            }
                            catch { /* best-effort */ }
                        }
                }

                var auditLog = new DevHub.Models.AuditLog
                {
                    UserId = moderatorId,
                    UserType = "Moderator",
                    Action = "Duyệt bài báo đang chờ duyệt",
                    EntityType = "Article",
                    EntityId = id,
                    NewValue = "Status -> PUBLISHED",
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

        [HttpPost("reject-pending/{id}")]
        public async Task<IActionResult> RejectPending(int id, [FromForm] string reason)
        {
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            try
            {
                await _articleService.HideArticleByModAsync(id, reason);

                var article = await _db.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.ArticleId == id);
                if (article != null)
                {
                    var recruiters = await _db.Recruiters
                        .Include(r => r.RecruiterNavigation)
                        .Where(r => r.CompanyId == article.CompanyId)
                        .ToListAsync();

                    foreach (var recruiter in recruiters)
                        {
                            try
                            {
                                await _notificationService.SendNotificationAsync(
                                    userId: recruiter.RecruiterId,
                                    userType: "RECRUITER",
                                    title: "Bài viết không được duyệt",
                                    message: $"Bài viết \"{article.Title}\" của bạn không được duyệt. Lý do: {reason}",
                                    type: "ARTICLE",
                                    severity: "warning",
                                    referenceId: id,
                                    referenceType: "Article"
                                );

                                if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email) && recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                                {
                                    var emailSubject = $"[DevHub] Bài viết không được duyệt: {article.Title}";
                                    var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                        "Bài viết không được duyệt",
                                        $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                        $"<p>Bài viết <strong>\"{article.Title}\"</strong> của bạn không được duyệt.</p>" +
                                        $"<p><strong>Lý do:</strong> {reason}</p>" +
                                        $"<p>Vui lòng đăng nhập vào hệ thống để chỉnh sửa lại bài viết.</p>" +
                                        $"<p>Cảm ơn bạn đã đồng hành cùng DevHub!</p>"
                                    );
                                    await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                                }
                            }
                            catch { /* best-effort */ }
                        }
                }

                var auditLog = new DevHub.Models.AuditLog
                {
                    UserId = moderatorId,
                    UserType = "Moderator",
                    Action = "Từ chối bài báo đang chờ duyệt",
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

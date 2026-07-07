using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Data;
using DevHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using DevHub.ViewModels.Moderator;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/company-approvals")]
    [Authorize(Roles = "Moderator")]
    [TypeFilter(typeof(DevHub.Filters.ModeratorTaskTypeAttribute), Arguments = new object[] { "COMPANY_APPROVAL" })]
    public class CompanyApprovalController : Controller
    {
        private readonly ItrecruitmentDbContext _db;

        public CompanyApprovalController(ItrecruitmentDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string companyName, DateTime? dateFrom, DateTime? dateTo, string sortOrder = "desc")
        {
            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);

            var query = _db.Companies
                .Where(c => c.Status == "PENDING" && c.ModeratorId == modId)
                .Select(c => new CompanyVerificationRequestViewModel
                {
                    LogId = c.CompanyId, // Dùng LogId lưu tạm CompanyId để View không đổi
                    RecruiterId = c.Recruiters.FirstOrDefault() != null ? c.Recruiters.FirstOrDefault().RecruiterId : 0,
                    CompanyName = c.CompanyName,
                    TaxCode = c.TaxCode ?? string.Empty,
                    BusinessLicenseUrl = c.BusinessLicenseUrl ?? string.Empty,
                    AdditionalDocumentsUrl = c.AdditionalDocumentsUrl ?? string.Empty,
                    RequestedAt = _db.AuditLogs.Where(a => a.Action == "VerificationRequest" && a.EntityId == (c.Recruiters.FirstOrDefault() != null ? c.Recruiters.FirstOrDefault().RecruiterId : 0)).Select(a => (DateTime?)a.CreatedAt).FirstOrDefault() ?? DateTime.UtcNow
                });

            if (!string.IsNullOrEmpty(companyName))
            {
                query = query.Where(q => q.CompanyName != null && q.CompanyName.Contains(companyName));
            }

            if (dateFrom.HasValue)
            {
                var from = dateFrom.Value.Date;
                query = query.Where(q => q.RequestedAt >= from);
            }

            if (dateTo.HasValue)
            {
                var to = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(q => q.RequestedAt <= to);
            }

            if (sortOrder == "asc")
            {
                query = query.OrderBy(q => q.RequestedAt);
            }
            else
            {
                query = query.OrderByDescending(q => q.RequestedAt);
            }

            var pendingRequests = await query.ToListAsync();

            ViewBag.CompanyName = companyName;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.SortOrder = sortOrder;

            return View("~/Views/Moderator/CompanyApproval/Index.cshtml", pendingRequests);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var company = await _db.Companies.Include(c => c.Recruiters).FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null || company.Status != "PENDING") return BadRequest("Invalid request.");

            company.IsVerified = true;
            company.Status = "APPROVED";

            var notification = new Notification
            {
                UserId = company.Recruiters.FirstOrDefault()?.RecruiterId ?? 0,
                UserType = "RECRUITER",
                Title = "Yêu cầu xác minh công ty đã được phê duyệt",
                Message = "Chúc mừng! Công ty của bạn đã được xác minh.",
                Type = "System",
                SeverityLevel = "info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _db.Notifications.Add(notification);

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);

            var auditLog = new AuditLog
            {
                UserId = modId,
                UserType = "Moderator",
                Action = "Duyệt công ty",
                EntityType = "Company",
                EntityId = company.CompanyId,
                OldValue = "Chờ duyệt",
                NewValue = "Đã duyệt",
                CreatedAt = DateTime.UtcNow
            };
            _db.AuditLogs.Add(auditLog);

            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromForm] string reason)
        {
            var company = await _db.Companies.Include(c => c.Recruiters).FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null || company.Status != "PENDING") return BadRequest("Invalid request.");

            company.Status = "REJECTED";
            // Lẽ ra cần có trường lưu lý do từ chối (như RejectedReason) trong Company, nếu chưa có tạm thời bỏ qua
            // company.RejectedReason = reason;

            var notification = new Notification
            {
                UserId = company.Recruiters.FirstOrDefault()?.RecruiterId ?? 0,
                UserType = "RECRUITER",
                Title = "Yêu cầu xác minh công ty bị từ chối",
                Message = $"Yêu cầu xác minh của bạn đã bị từ chối. Lý do: {reason}",
                Type = "System",
                SeverityLevel = "warning",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _db.Notifications.Add(notification);

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);

            var auditLog = new AuditLog
            {
                UserId = modId,
                UserType = "Moderator",
                Action = "Từ chối công ty",
                EntityType = "Company",
                EntityId = company.CompanyId,
                OldValue = "Chờ duyệt",
                NewValue = "Từ chối (Lý do: " + reason + ")",
                CreatedAt = DateTime.UtcNow
            };
            _db.AuditLogs.Add(auditLog);

            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}

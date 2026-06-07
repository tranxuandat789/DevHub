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
    public class CompanyApprovalController : Controller
    {
        private readonly ItrecruitmentDbContext _db;

        public CompanyApprovalController(ItrecruitmentDbContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var pendingRequests = await _db.AuditLogs
                .Where(a => a.Action == "VerificationRequest" && a.EntityType == "recruiter_profile" && a.OldValue == null)
                .Join(_db.Recruiters, 
                    a => a.EntityId, 
                    r => r.RecruiterId, 
                    (a, r) => new CompanyVerificationRequestViewModel
                    {
                        LogId = a.LogId,
                        RecruiterId = r.RecruiterId,
                        CompanyName = r.CompanyName,
                        TaxCode = r.TaxCode,
                        BusinessLicenseUrl = r.BusinessLicenseUrl,
                        AdditionalDocumentsUrl = r.AdditionalDocumentsUrl,
                        RequestedAt = a.CreatedAt
                    })
                .ToListAsync();

            return View("~/Views/Moderator/CompanyApproval/Index.cshtml", pendingRequests);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var log = await _db.AuditLogs.FindAsync(id);
            if (log == null || log.OldValue != null) return BadRequest("Invalid request.");

            var recruiter = await _db.Recruiters.FindAsync(log.EntityId);
            if (recruiter != null)
            {
                recruiter.IsVerified = true;
            }

            log.OldValue = "Approved";

            var notification = new Notification
            {
                UserId = log.EntityId.Value,
                UserType = "RECRUITER",
                Title = "Yêu cầu xác minh công ty đã được phê duyệt",
                Message = "Chúc mừng! Công ty của bạn đã được xác minh.",
                Type = "System",
                SeverityLevel = "info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _db.Notifications.Add(notification);

            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromForm] string reason)
        {
            var log = await _db.AuditLogs.FindAsync(id);
            if (log == null || log.OldValue != null) return BadRequest("Invalid request.");

            log.OldValue = "Rejected";

            var notification = new Notification
            {
                UserId = log.EntityId.Value,
                UserType = "RECRUITER",
                Title = "Yêu cầu xác minh công ty bị từ chối",
                Message = $"Yêu cầu xác minh của bạn đã bị từ chối. Lý do: {reason}",
                Type = "System",
                SeverityLevel = "warning",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _db.Notifications.Add(notification);

            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}

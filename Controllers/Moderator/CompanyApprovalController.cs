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
        private readonly DevHub.Services.Interfaces.INotificationService _notificationService;
        private readonly DevHub.Helpers.EmailHelper _emailHelper;

        public CompanyApprovalController(ItrecruitmentDbContext db, DevHub.Services.Interfaces.INotificationService notificationService, DevHub.Helpers.EmailHelper emailHelper)
        {
            _db = db;
            _notificationService = notificationService;
            _emailHelper = emailHelper;
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

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var company = await _db.Companies
                .Include(c => c.Recruiters)
                .FirstOrDefaultAsync(c => c.CompanyId == id);
            
            if (company == null) return NotFound();

            return View("~/Views/Moderator/CompanyApproval/Details.cshtml", company);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var company = await _db.Companies.Include(c => c.Recruiters).FirstOrDefaultAsync(c => c.CompanyId == id);
            if (company == null || company.Status != "PENDING") return BadRequest("Invalid request.");

            company.IsVerified = true;
            company.Status = "APPROVED";

            // Gửi thông báo cho tất cả Recruiter của công ty
            var recruiters = company.Recruiters.ToList();
            foreach (var r in recruiters)
            {
                var userAccount = await _db.UserAccounts.FindAsync(r.RecruiterId);
                if (userAccount != null)
                {
                    // In-app
                    await _notificationService.SendNotificationAsync(
                        userAccount.UserId,
                        "RECRUITER",
                        "COMPANY_APPROVED",
                        $"Chúc mừng! Hồ sơ công ty {company.CompanyName} đã được xác minh.",
                        "/recruiter/profile"
                    );

                    // Email
                    if (userAccount.EmailNotificationsEnabled)
                    {
                        string subject = "DevHub - Hồ sơ công ty đã được xác minh";
                        string body = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2 style='color: #4CAF50;'>Công ty của bạn đã được xác minh!</h2>
                            <p>Chào {r.FullName},</p>
                            <p>Hồ sơ công ty <strong>{company.CompanyName}</strong> của bạn đã được kiểm duyệt viên xác minh thành công.</p>
                            <p>Bây giờ bạn có thể bắt đầu đăng tin tuyển dụng.</p>
                            <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                        </div>";
                        try
                        {
                            await _emailHelper.SendEmailAsync(userAccount.Email, subject, body);
                        }
                        catch (Exception ex)
                        {
                            // Ghi log lỗi gửi email nếu cần
                            Console.WriteLine("Lỗi gửi email: " + ex.Message);
                        }
                    }
                }
            }

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

            // Gửi thông báo cho tất cả Recruiter của công ty
            var recruiters = company.Recruiters.ToList();
            foreach (var r in recruiters)
            {
                var userAccount = await _db.UserAccounts.FindAsync(r.RecruiterId);
                if (userAccount != null)
                {
                    // In-app
                    await _notificationService.SendNotificationAsync(
                        userAccount.UserId,
                        "RECRUITER",
                        "COMPANY_REJECTED",
                        $"Yêu cầu xác minh công ty {company.CompanyName} bị từ chối.",
                        "/recruiter/profile"
                    );

                    // Email
                    if (userAccount.EmailNotificationsEnabled)
                    {
                        string subject = "DevHub - Yêu cầu xác minh công ty bị từ chối";
                        string body = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2 style='color: #F44336;'>Xác minh công ty thất bại</h2>
                            <p>Chào {r.FullName},</p>
                            <p>Yêu cầu xác minh công ty <strong>{company.CompanyName}</strong> của bạn không được phê duyệt với lý do sau:</p>
                            <blockquote style='border-left: 4px solid #F44336; padding-left: 10px; color: #555;'>
                                {reason}
                            </blockquote>
                            <p>Vui lòng cập nhật lại giấy tờ và thông tin để được duyệt.</p>
                            <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                        </div>";
                        try
                        {
                            await _emailHelper.SendEmailAsync(userAccount.Email, subject, body);
                        }
                        catch (Exception ex)
                        {
                            // Ghi log lỗi gửi email nếu cần
                            Console.WriteLine("Lỗi gửi email: " + ex.Message);
                        }
                    }
                }
            }

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

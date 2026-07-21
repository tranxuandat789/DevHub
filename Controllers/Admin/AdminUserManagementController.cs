//KienHM-5/7/2026

using DevHub.Data;
using DevHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Controllers.Admin
{
    [Route("admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserManagementController : Controller
    {
        private readonly ItrecruitmentDbContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public AdminUserManagementController(ItrecruitmentDbContext context, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("")]
        [HttpGet("/AdminUser")]
        public async Task<IActionResult> Index([FromQuery] DevHub.ViewModels.Moderator.UserManagementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Page = 1;
            }

            string search = model.Search ?? "";
            string role = model.Role ?? "";
            string sort = model.Sort ?? "";
            int page = model.Page;

            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.Sort = sort;

            int pageSize = 10;
            var query = _context.UserAccounts
                .Include(u => u.Candidate)
                .Include(u => u.Recruiter)
                // Logic phân tách: Giao diện này dành riêng cho User thường (Candidate/Recruiter). Việc quản lý tài khoản Moderator được cấp giao diện riêng biệt để tuân thủ tính chuyên biệt theo BR-MOD-02.
                .Where(u => u.UserType != "Admin" && u.UserType != "Moderator")
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(search) 
                    || (u.Candidate != null && u.Candidate.FullName.ToLower().Contains(search))
                    || (u.Recruiter != null && u.Recruiter.Company.CompanyName.ToLower().Contains(search))
                    || (u.Recruiter != null && u.Recruiter.FullName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.UserType.ToLower() == role.ToLower());
            }

            switch (sort)
            {
                case "name_asc":
                    query = query.OrderBy(u => u.Candidate != null ? u.Candidate.FullName : (u.Recruiter != null ? u.Recruiter.FullName : u.Email));
                    break;
                case "name_desc":
                    query = query.OrderByDescending(u => u.Candidate != null ? u.Candidate.FullName : (u.Recruiter != null ? u.Recruiter.FullName : u.Email));
                    break;
                case "spending_desc":
                    query = query.OrderByDescending(u => u.Recruiter != null ? u.Recruiter.Company.TotalSpent : 0);
                    break;
                case "spending_asc":
                    query = query.OrderBy(u => u.Recruiter != null ? u.Recruiter.Company.TotalSpent : 0);
                    break;
                default:
                    query = query.OrderByDescending(u => u.CreatedAt);
                    break;
            }

            var totalItems = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Admin/AdminUserManagement/Index.cshtml", users);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.UserAccounts
                .Include(u => u.Candidate)
                .Include(u => u.Recruiter)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var history = await _context.AuditLogs
                .Where(a => a.EntityId == id && a.EntityType == "UserAccount")
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            
            ViewBag.History = history;

            var moderatorIds = history.Where(h => h.UserId.HasValue).Select(h => h.UserId!.Value).Distinct().ToList();
            var moderatorNames = await _context.UserAccounts
                .Where(u => moderatorIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId, u => u.Candidate != null ? u.Candidate.FullName ?? u.Email
                    : u.Recruiter != null ? u.Recruiter.FullName ?? u.Email
                    : u.Email);
            ViewBag.ModeratorNames = moderatorNames;

            return View("~/Views/Admin/AdminUserManagement/Details.cshtml", user);
        }

        public class ToggleStatusRequest
        {
            public string? Reason { get; set; }
        }

        [HttpPost("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id, [FromBody] ToggleStatusRequest request)
        {
            var user = await _context.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Người dùng không tồn tại." });
            }

            if (user.UserType == "Admin" || user.UserType == "Moderator")
            {
                return BadRequest(new { success = false, message = "Không thể thay đổi trạng thái của Admin hoặc Moderator." });
            }

            bool oldStatus = user.IsActive ?? true;
            user.IsActive = !oldStatus;
            
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int adminId = 0;
            int.TryParse(userIdStr, out adminId);

            string oldStatusVi = oldStatus ? "Hoạt động" : "Khóa";
            string newStatusVi = (user.IsActive == true) ? "Hoạt động" : "Khóa";
            string newValueStr = newStatusVi + (string.IsNullOrEmpty(request?.Reason) ? "" : $" (Lý do: {request.Reason})");

            var auditLog = new AuditLog
            {
                Action = user.IsActive == true ? "Mở khóa tài khoản" : "Khóa tài khoản",
                EntityId = user.UserId,
                EntityType = "UserAccount",
                OldValue = oldStatusVi,
                NewValue = newValueStr,
                UserId = adminId,
                UserType = "Admin",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            if (user.IsActive == false && !string.IsNullOrEmpty(request?.Reason) && !string.IsNullOrEmpty(user.Email))
            {
                try 
                {
                    // BR-NTF-03 (Email Delivery): Gửi thông báo tự động qua email khi tài khoản người dùng bị Admin khóa.
                    var emailHelper = new DevHub.Helpers.EmailHelper(_config);
                    string subject = "Thông báo: Tài khoản của bạn đã bị khóa";
                    string content = $@"
                        <p>Chào bạn,</p>
                        <p>Tài khoản của bạn trên DevHub đã bị khóa bởi quản trị viên.</p>
                        <div style='background:#fff3cd;border-left:4px solid #ffc107;padding:16px;border-radius:8px;margin:16px 0;'>
                            <p style='margin:0;color:#856404;'><strong>Lý do:</strong> {request.Reason}</p>
                        </div>
                        <p>Vui lòng liên hệ với ban quản trị nếu bạn có thắc mắc.</p>";
                    string body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Tài khoản bị khóa", content);
                    await emailHelper.SendEmailAsync(user.Email, subject, body);
                } 
                catch (System.Exception ex) 
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi gửi email: {ex.Message}");
                }
            }

            return Ok(new { success = true, isActive = user.IsActive, message = user.IsActive == true ? "Đã mở khóa tài khoản thành công!" : "Đã khóa tài khoản thành công!" });
        }
    }
}

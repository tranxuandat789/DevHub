using DevHub.Data;
using DevHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/users")]
    [Authorize(Roles = "Moderator")]
    public class ModeratorUserManagementController : Controller
    {
        private readonly ItrecruitmentDbContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public ModeratorUserManagementController(ItrecruitmentDbContext context, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("")]
        [HttpGet("/ModeratorUser")]
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
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(search) 
                    || (u.Candidate != null && u.Candidate.FullName.ToLower().Contains(search))
                    || (u.Recruiter != null && u.Recruiter.CompanyName.ToLower().Contains(search))
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
                    query = query.OrderByDescending(u => u.Recruiter != null ? u.Recruiter.TotalSpent : 0);
                    break;
                case "spending_asc":
                    query = query.OrderBy(u => u.Recruiter != null ? u.Recruiter.TotalSpent : 0);
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

            return View("~/Views/Moderator/ModeratorUserManagement/Index.cshtml", users);
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

            return View("~/Views/Moderator/ModeratorUserManagement/Details.cshtml", user);
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
                return BadRequest(new { success = false, message = "Không thể thay đổi trạng thái của Admin/Moderator." });
            }

            bool oldStatus = user.IsActive ?? true;
            user.IsActive = !oldStatus;
            
            // Log to AuditLog
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int moderatorId = 0;
            int.TryParse(userIdStr, out moderatorId);

            var auditLog = new AuditLog
            {
                Action = user.IsActive == true ? "Unlock User Account" : "Lock User Account",
                EntityId = user.UserId,
                EntityType = "UserAccount",
                OldValue = oldStatus.ToString(),
                NewValue = user.IsActive.ToString() + (string.IsNullOrEmpty(request?.Reason) ? "" : $" (Reason: {request.Reason})"),
                UserId = moderatorId,
                UserType = "Moderator",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            if (user.IsActive == false && !string.IsNullOrEmpty(request?.Reason) && !string.IsNullOrEmpty(user.Email))
            {
                try 
                {
                    var emailHelper = new DevHub.Helpers.EmailHelper(_config);
                    string subject = "Thông báo: Tài khoản của bạn đã bị khóa";
                    string body = $"Chào bạn,<br/><br/>Tài khoản của bạn trên DevHub đã bị khóa bởi quản trị viên.<br/><strong>Lý do:</strong> {request.Reason}<br/><br/>Vui lòng liên hệ với ban quản trị nếu bạn có thắc mắc.";
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

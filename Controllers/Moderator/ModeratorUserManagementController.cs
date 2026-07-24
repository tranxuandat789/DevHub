// Author: PhongDH
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
        // Truy vấn, tìm kiếm, lọc và phân trang danh sách người dùng trong hệ thống (ngoại trừ quản trị viên).
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

            return View("~/Views/Moderator/ModeratorUserManagement/Index.cshtml", users);
        }

        [HttpGet("details/{id}")]
        // Lấy thông tin chi tiết của người dùng và lịch sử các hành động quản trị (Audit Log) liên quan.
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

            // Lấy tên moderator cho từng log
            var moderatorIds = history.Where(h => h.UserId.HasValue).Select(h => h.UserId!.Value).Distinct().ToList();
            var moderatorNames = await _context.UserAccounts
                .Where(u => moderatorIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId, u => u.Candidate != null ? u.Candidate.FullName ?? u.Email
                    : u.Recruiter != null ? u.Recruiter.FullName ?? u.Email
                    : u.Email);
            ViewBag.ModeratorNames = moderatorNames;

            return View("~/Views/Moderator/ModeratorUserManagement/Details.cshtml", user);
        }

    }
}

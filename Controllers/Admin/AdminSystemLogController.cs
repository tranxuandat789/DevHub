using DevHub.Data;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers.Admin
{
    [Route("AdminSystemLog")]
    [Authorize(Roles = "Admin")]
    public class AdminSystemLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ItrecruitmentDbContext _context;

        public AdminSystemLogController(IAuditLogService auditLogService, ItrecruitmentDbContext context)
        {
            _auditLogService = auditLogService;
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? userType = null, DateTime? startDate = null, DateTime? endDate = null, string? entityType = null, int page = 1)
        {
            var allLogs = await _auditLogService.GetLogsAsync();
            var logs = allLogs;

            if (!string.IsNullOrEmpty(userType))
            {
                logs = logs.Where(l => l.UserType == userType).ToList();
            }
            
            if (!string.IsNullOrEmpty(entityType))
            {
                logs = logs.Where(l => l.EntityType == entityType).ToList();
            }

            if (startDate.HasValue)
            {
                logs = logs.Where(l => l.CreatedAt >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.AddDays(1).AddTicks(-1);
                logs = logs.Where(l => l.CreatedAt <= end).ToList();
            }

            int pageSize = 10;
            int totalItems = logs.Count();
            int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            
            var paginatedLogs = logs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var entityInfos = new Dictionary<string, string>();

            var userAccountLogs = paginatedLogs.Where(l => l.EntityType == "UserAccount" && l.EntityId.HasValue).Select(l => l.EntityId.Value).ToList();
            if (userAccountLogs.Any())
            {
                var users = await _context.UserAccounts
                    .Include(u => u.Candidate)
                    .Include(u => u.Recruiter)
                    .Include(u => u.Admin)
                    .Where(u => userAccountLogs.Contains(u.UserId))
                    .ToListAsync();
                
                foreach (var u in users)
                {
                    string name = u.Candidate?.FullName ?? u.Recruiter?.FullName ?? u.Admin?.FullName ?? "Không có tên";
                    entityInfos[$"UserAccount_{u.UserId}"] = $"{name} ({u.Email})";
                }
            }

            var techLogs = paginatedLogs.Where(l => l.EntityType == "CommonTechnology" && l.EntityId.HasValue).Select(l => l.EntityId.Value).ToList();
            if (techLogs.Any())
            {
                var techs = await _context.CommonTechnologies
                    .Where(t => techLogs.Contains(t.TechId))
                    .ToListAsync();
                
                foreach (var t in techs)
                {
                    entityInfos[$"CommonTechnology_{t.TechId}"] = t.TechName;
                }
            }

            var blogLogs = paginatedLogs.Where(l => l.EntityType == "BlogPost" && l.EntityId.HasValue).Select(l => l.EntityId.Value).ToList();
            if (blogLogs.Any())
            {
                var blogs = await _context.BlogPosts
                    .Where(b => blogLogs.Contains(b.BlogId))
                    .ToListAsync();
                
                foreach (var b in blogs)
                {
                    entityInfos[$"BlogPost_{b.BlogId}"] = string.IsNullOrEmpty(b.Title) ? "Không có tiêu đề" : b.Title;
                }
            }

            // Fallback cho các entity đã bị xóa khỏi database
            foreach (var log in paginatedLogs.Where(l => l.EntityId.HasValue))
            {
                string key = $"{log.EntityType}_{log.EntityId.Value}";
                if (!entityInfos.ContainsKey(key))
                {
                    entityInfos[key] = $"{log.EntityType} đã bị xóa (ID: {log.EntityId.Value})";
                }
            }

            ViewBag.EntityInfos = entityInfos;

            var performerIds = paginatedLogs.Where(l => l.UserId.HasValue).Select(l => l.UserId.Value).Distinct().ToList();
            var performerNames = new Dictionary<int, string>();
            if (performerIds.Any())
            {
                var performers = await _context.UserAccounts
                    .Include(u => u.Admin)
                    .Include(u => u.Candidate)
                    .Include(u => u.Recruiter)
                    .Where(u => performerIds.Contains(u.UserId))
                    .ToListAsync();
                
                foreach (var p in performers)
                {
                    string pName = p.Admin?.FullName ?? p.Candidate?.FullName ?? p.Recruiter?.FullName ?? p.Email ?? "Unknown";
                    performerNames[p.UserId] = pName;
                }
            }
            ViewBag.PerformerNames = performerNames;

            ViewBag.SelectedUserType = userType;
            ViewBag.SelectedEntityType = entityType;
            ViewBag.SelectedStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedEndDate = endDate?.ToString("yyyy-MM-dd");
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            
            var entityTypes = allLogs.Where(l => !string.IsNullOrEmpty(l.EntityType))
                                     .Select(l => l.EntityType)
                                     .Distinct()
                                     .ToList();
            ViewBag.EntityTypes = entityTypes;

            return View("~/Views/Admin/AdminSystemLog/Index.cshtml", paginatedLogs);
        }
    }
}

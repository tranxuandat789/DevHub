using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin
{
    [Route("AdminSystemLog")]
    [Authorize(Roles = "Admin")]
    public class AdminSystemLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AdminSystemLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
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

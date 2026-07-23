//KienHM-20/6/2026

using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/packages")]
    [Authorize(Roles = "Moderator,ADMIN,MODERATOR")]
    public class PackageManagementController : Controller
    {
        private readonly IServicePackageService _packageService;
        private readonly DevHub.Data.ItrecruitmentDbContext _db;

        public PackageManagementController(IServicePackageService packageService, DevHub.Data.ItrecruitmentDbContext db)
        {
            _packageService = packageService;
            _db = db;
        }

        [HttpGet("")]
        [HttpGet("/ModeratorPackage")]
        public async Task<IActionResult> Index([FromQuery] string searchTerm = "", [FromQuery] string sortOrder = "newest", [FromQuery] string statusFilter = "all", [FromQuery] int page = 1)
        {
            int pageSize = 10;
            var result = await _packageService.GetAllPackagesAsync(searchTerm, statusFilter, sortOrder, page, pageSize);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;

            return View("~/Views/Moderator/PackageManagement/Index.cshtml", result.Items);
        }

        [HttpGet("create")]
        [HttpGet("/ModeratorPackage/Create")]
        public IActionResult Create()
        {
            return View("~/Views/Moderator/PackageManagement/Create.cshtml", new ServicePackage());
        }

        [HttpPost("create")]
        [HttpPost("/ModeratorPackage/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServicePackage package)
        {
            if (ModelState.IsValid)
            {
                await _packageService.CreatePackageAsync(package);

                var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(modIdClaim, out int modId);
                var userType = User.IsInRole("Admin") ? "Admin" : "Moderator";
                _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                    UserId = modId, 
                    UserType = userType, 
                    Action = "Thêm gói dịch vụ", 
                    EntityType = "ServicePackage", 
                    EntityId = package.ServiceId, 
                    OldValue = "Không", 
                    NewValue = package.PackageName, 
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });
                await _db.SaveChangesAsync();

                TempData["SuccessMsg"] = "Đã tạo gói dịch vụ mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Moderator/PackageManagement/Create.cshtml", package);
        }

        [HttpGet("edit/{id}")]
        [HttpGet("/ModeratorPackage/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var package = await _packageService.GetByIdAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return View("~/Views/Moderator/PackageManagement/Edit.cshtml", package);
        }

        [HttpPost("edit/{id}")]
        [HttpPost("/ModeratorPackage/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServicePackage package)
        {
            if (id != package.ServiceId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _packageService.UpdatePackageAsync(package);

                var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(modIdClaim, out int modId);
                var userType = User.IsInRole("Admin") ? "Admin" : "Moderator";
                _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                    UserId = modId, 
                    UserType = userType, 
                    Action = "Sửa gói dịch vụ", 
                    EntityType = "ServicePackage", 
                    EntityId = id, 
                    OldValue = "Tồn tại", 
                    NewValue = package.PackageName, 
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });
                await _db.SaveChangesAsync();

                TempData["SuccessMsg"] = "Đã cập nhật gói dịch vụ thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Moderator/PackageManagement/Edit.cshtml", package);
        }

        [HttpPost("toggle-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, [FromForm] bool activate)
        {
            // Logic tương quan với BR-MOD-03: Sử dụng cơ chế soft-delete (ẩn/hiện) gói dịch vụ thay vì xóa cứng (hard-delete) để bảo toàn lịch sử giao dịch (PackageTransaction) và các gói Recruiter đang dùng (CompanyPackageHistory).
            var success = await _packageService.ToggleStatusAsync(id, activate);
            if (success)
            {
                var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(modIdClaim, out int modId);
                var userType = User.IsInRole("Admin") ? "Admin" : "Moderator";
                _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                    UserId = modId, 
                    UserType = userType, 
                    Action = "Đổi trạng thái gói dịch vụ", 
                    EntityType = "ServicePackage", 
                    EntityId = id, 
                    OldValue = !activate ? "Hoạt động" : "Khóa", 
                    NewValue = activate ? "Hoạt động" : "Khóa", 
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });
                await _db.SaveChangesAsync();

                TempData["SuccessMsg"] = $"Đã thay đổi trạng thái gói dịch vụ #{id} thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi khi cập nhật trạng thái gói dịch vụ.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

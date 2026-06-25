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

        public PackageManagementController(IServicePackageService packageService)
        {
            _packageService = packageService;
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
                TempData["SuccessMsg"] = "Đã cập nhật gói dịch vụ thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Moderator/PackageManagement/Edit.cshtml", package);
        }

        [HttpPost("toggle-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, [FromForm] bool activate)
        {
            var success = await _packageService.ToggleStatusAsync(id, activate);
            if (success)
            {
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

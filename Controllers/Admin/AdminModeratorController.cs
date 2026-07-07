// 04/06/2026-DatTX
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Admin;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DevHub.Controllers.Admin
{
    [Authorize(AuthenticationSchemes = "AdminCookies", Roles = "Admin")]
    [Route("AdminModerator")]
    public class AdminModeratorController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IModAssignmentService _modAssignmentService;
        private readonly IAssignModeratorService _assignModeratorService;

        public AdminModeratorController(
            IAdminService adminService,
            IModAssignmentService modAssignmentService,
            IAssignModeratorService assignModeratorService)
        {
            _adminService            = adminService;
            _modAssignmentService    = modAssignmentService;
            _assignModeratorService  = assignModeratorService;
        }

        // GET: /AdminModerator
        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] string searchTerm = "", [FromQuery] string sortOrder = "newest", [FromQuery] string statusFilter = "all", [FromQuery] int page = 1)
        {
            int pageSize = 5; 
            
            var result = await _adminService.GetModeratorListAsync(searchTerm, statusFilter, page, pageSize);
            var moderators = result.Items;
            var totalCount = result.TotalCount;

            if (sortOrder == "oldest")
            {
                moderators = moderators.OrderBy(m => m.AdminNavigation.CreatedAt).ToList();
            }
            else
            {
                moderators = moderators.OrderByDescending(m => m.AdminNavigation.CreatedAt).ToList();
            }

            var moderatorTaskTypes = await _assignModeratorService.GetWorkloadByTaskTypeAsync("COMPANY_APPROVAL");
            var jobTaskTypes       = await _assignModeratorService.GetWorkloadByTaskTypeAsync("JOB_POST");
            var reviewTaskTypes    = await _assignModeratorService.GetWorkloadByTaskTypeAsync("REVIEW");

            var taskTypeMap = moderatorTaskTypes
                .Concat(jobTaskTypes)
                .Concat(reviewTaskTypes)
                .ToDictionary(m => m.ModeratorId, m => m.TaskType);

            var viewModel = new ModeratorListViewModel
            {
                Items = moderators.Select(m => new ModeratorListItemDto
                {
                    AdminId  = m.AdminId,
                    Username = m.Username,
                    Email    = m.AdminNavigation?.Email ?? "",
                    FullName = m.FullName ?? "",
                    IsActive = m.AdminNavigation?.IsActive ?? false,
                    CreatedAt= m.AdminNavigation?.CreatedAt,
                    TaskType = taskTypeMap.ContainsKey(m.AdminId) ? taskTypeMap[m.AdminId] : null
                }).ToList(),
                Search = searchTerm,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Filter = statusFilter
            };

            ViewBag.SortOrder = sortOrder;
            ViewBag.StatusFilter = statusFilter;

            return View("~/Views/Admin/AdminModerator/Index.cshtml", viewModel);
        }

        // GET: /AdminModerator/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/AdminModerator/Create.cshtml", new CreateModeratorViewModel());
        }

        // POST: /AdminModerator/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateModeratorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/AdminModerator/Create.cshtml", model);
            }

            var result = await _adminService.CreateModeratorAsync(model.Email, model.Password, model.Username, model.FullName);
            
            if (result.Success)
            {
                // Lấy moderator vừa tạo để gán task type
                var newMod = await _adminService.GetModeratorByIdAsync(
                    (await _adminService.GetModeratorListAsync("", "active", 1, 1)).Items
                    .FirstOrDefault(m => m.Username == model.Username)?.AdminId ?? 0);

                if (newMod != null && !string.IsNullOrEmpty(model.TaskType))
                {
                    var currentAdminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(currentAdminIdStr, out int currentAdminId))
                        await _assignModeratorService.SetTaskTypeAsync(newMod.AdminId, model.TaskType, currentAdminId);
                }

                TempData["SuccessMsg"] = "Lưu Moderator thành công! Hệ thống đã ghi nhận.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("Email", result.Message);
                return View("~/Views/Admin/AdminModerator/Create.cshtml", model);
            }
        }

        // GET: /AdminModerator/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var admin = await _adminService.GetModeratorByIdAsync(id);
            if (admin == null || admin.Role != "MODERATOR")
            {
                return NotFound();
            }

            var currentTaskType = await _assignModeratorService.GetTaskTypeAsync(id);

            var model = new EditModeratorViewModel
            {
                AdminId         = admin.AdminId,
                Username        = admin.Username,
                Email           = admin.AdminNavigation?.Email ?? "",
                FullName        = admin.FullName ?? "",
                CurrentTaskType = currentTaskType
            };

            return View("~/Views/Admin/AdminModerator/Edit.cshtml", model);
        }

        // POST: /AdminModerator/Edit/{id}
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditModeratorViewModel model)
        {
            if (id != model.AdminId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/AdminModerator/Edit.cshtml", model);
            }

            var result = await _adminService.UpdateModeratorAsync(model.AdminId, model.FullName, model.Username);
            
            if (result.Success)
            {
                // Cập nhật task type nếu admin đã chọn
                if (!string.IsNullOrEmpty(model.TaskType))
                {
                    var currentAdminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(currentAdminIdStr, out int currentAdminId))
                        await _assignModeratorService.SetTaskTypeAsync(model.AdminId, model.TaskType, currentAdminId);
                }

                TempData["SuccessMsg"] = $"Đã cập nhật thông tin Moderator #{model.AdminId} thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("Username", result.Message);
                return View("~/Views/Admin/AdminModerator/Edit.cshtml", model);
            }
        }

        // POST: /AdminModerator/ToggleStatus/{id}
        [HttpPost("ToggleStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, [FromForm] bool activate)
        {
            bool success;
            if (activate)
            {
                success = await _adminService.ReactivateModeratorAsync(id);
            }
            else
            {
                success = await _adminService.DeactivateModeratorAsync(id);
            }

            if (success)
            {
                TempData["SuccessMsg"] = $"Đã cập nhật trạng thái Moderator #{id} thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi hệ thống khi cập nhật trạng thái.";
            }

            return RedirectToAction("Index");
        }
    }
}

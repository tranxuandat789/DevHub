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
        private readonly DevHub.Repositories.Interfaces.IIndustryAssignmentRepository _industryRepo;
        private readonly DevHub.Repositories.Interfaces.ICompanyRepository _companyRepo;

        public AdminModeratorController(
            IAdminService adminService,
            IModAssignmentService modAssignmentService,
            IAssignModeratorService assignModeratorService,
            DevHub.Repositories.Interfaces.IIndustryAssignmentRepository industryRepo,
            DevHub.Repositories.Interfaces.ICompanyRepository companyRepo)
        {
            _adminService            = adminService;
            _modAssignmentService    = modAssignmentService;
            _assignModeratorService  = assignModeratorService;
            _industryRepo            = industryRepo;
            _companyRepo             = companyRepo;
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

            var industryAssignments = await _industryRepo.GetAllByTaskTypeAsync(null); // Actually, I should probably just fetch all of them
            var industriesMap = industryAssignments.GroupBy(a => a.ModeratorId)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Industry).ToList());

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
                    TaskType = taskTypeMap.ContainsKey(m.AdminId) ? taskTypeMap[m.AdminId] : null,
                    Industries = industriesMap.ContainsKey(m.AdminId) ? industriesMap[m.AdminId] : new List<string>()
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
                if (result.AdminId > 0 && !string.IsNullOrEmpty(model.TaskType))
                {
                    var currentAdminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(currentAdminIdStr, out int currentAdminId))
                        await _assignModeratorService.SetTaskTypeAsync(result.AdminId, model.TaskType, currentAdminId);
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

        // GET: /AdminModerator/{id}/industry-settings
        [HttpGet("{id}/industry-settings")]
        public async Task<IActionResult> IndustrySettings(int id)
        {
            var taskType = await _assignModeratorService.GetTaskTypeAsync(id);
            if (string.IsNullOrEmpty(taskType))
            {
                return Json(new { error = "Moderator này chưa được phân công sub-role (Task Type)." });
            }

            var currentIndustries = await _industryRepo.GetIndustriesAsync(id);
            
            // Get all distinct industries from companies
            var allCompanies = await _companyRepo.GetAllAsync();
            var distinctIndustries = allCompanies
                .Where(c => !string.IsNullOrWhiteSpace(c.Industry))
                .Select(c => c.Industry.Trim())
                .Distinct()
                .OrderBy(i => i)
                .ToList();

            // Find owners of these industries for this task type
            var assignments = await _industryRepo.GetAllByTaskTypeAsync(taskType);
            
            var industryData = distinctIndustries.Select(ind => {
                var assignment = assignments.FirstOrDefault(a => a.Industry == ind);
                return new {
                    Name = ind,
                    IsChecked = currentIndustries.Contains(ind),
                    OwnerId = assignment?.ModeratorId,
                    OwnerName = assignment?.Moderator?.FullName ?? assignment?.Moderator?.Username
                };
            }).ToList();

            return Json(new {
                taskType = taskType,
                industries = industryData
            });
        }

        // POST: /AdminModerator/{id}/industry-settings
        [HttpPost("{id}/industry-settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIndustrySettings(int id, [FromForm] string taskType, [FromForm] System.Collections.Generic.List<string> selectedIndustries)
        {
            var currentAdminIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(currentAdminIdStr, out int currentAdminId))
            {
                await _industryRepo.SetIndustriesAsync(id, taskType, selectedIndustries, currentAdminId);
                TempData["SuccessMsg"] = "Đã cập nhật phân công ngành thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Không thể xác định Admin ID.";
            }

            return RedirectToAction("Edit", new { id = id });
        }
    }
}

//KienHM-03/06/2026
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers.Moderator
{
    [Route("ModeratorTechStack")]
    [Authorize(Roles = "Moderator")]
    public class TechStackController : Controller
    {
        // 1. Khai báo Service (Business Logic Layer)
        private readonly DevHub.Services.Interfaces.ICommonTechnologyService _techService;
        private readonly DevHub.Data.ItrecruitmentDbContext _db;

        public TechStackController(DevHub.Services.Interfaces.ICommonTechnologyService techService, DevHub.Data.ItrecruitmentDbContext db)
        {
            _techService = techService;
            _db = db;
        }

        // 2. Action Index hiển thị danh sách (có hỗ trợ tìm kiếm và lọc)
        [HttpGet("")]
        public async Task<IActionResult> Index(string keyword = "", string status = "", string category = "", string sortOrder = "asc", int page = 1)
        {
            var techs = await _techService.GetAllTechsAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                techs = techs.Where(t => t.TechName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool isActive = status == "1";
                techs = techs.Where(t => t.IsActive == isActive).ToList();
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                techs = techs.Where(t => t.Category == category).ToList();
            }

            if (sortOrder == "desc")
            {
                techs = techs
                    .OrderByDescending(t => t.DisplayOrder)
                    .ThenBy(t => t.TechName)
                    .ToList();
            }
            else
            {
                techs = techs
                    .OrderBy(t => t.DisplayOrder)
                    .ThenBy(t => t.TechName)
                    .ToList();
            }

            int pageSize = 10;
            int totalItems = techs.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedTechs = techs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            // Get job counts
            var jobCounts = await _db.CommonTechnologies
                .Select(t => new { t.TechId, Count = t.Jobs.Count })
                .ToDictionaryAsync(x => x.TechId, x => x.Count);
            ViewBag.JobCounts = jobCounts;

            ViewData["Keyword"] = keyword;
            ViewData["Status"] = status;
            ViewData["Category"] = category;
            ViewData["SortOrder"] = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Moderator/TechStack/Index.cshtml", pagedTechs);
        }

        // 3. Action hiển thị form Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Moderator/TechStack/Create.cshtml");
        }

        // 4. Action xử lý POST khi lưu form Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] string techName, [FromForm] string category, [FromForm] bool isActive = true, [FromForm] int displayOrder = 1)
        {
            if (string.IsNullOrWhiteSpace(techName))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên công nghệ.";
                return View("~/Views/Moderator/TechStack/Create.cshtml");
            }

            // Logic tương quan với BR-PRF-02: Ngăn chặn tạo trùng lặp tên công nghệ trong hệ thống DB master (để đảm bảo tính nhất quán khi Candidate/Recruiter thêm tech).
            var allTechs = await _techService.GetAllTechsAsync();
            if (allTechs.Any(t => t.TechName.Equals(techName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.ErrorMessage = $"Lỗi: Công nghệ '{techName}' đã tồn tại trong hệ thống.";
                return View("~/Views/Moderator/TechStack/Create.cshtml");
            }

            var newTech = new DevHub.Models.CommonTechnology
            {
                TechName = techName.Trim(),
                Category = string.IsNullOrWhiteSpace(category) ? "Chưa phân loại" : category.Trim(),
                IsActive = isActive
            };
            
            await _techService.AddTechAsync(newTech);

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                UserId = modId, UserType = "Moderator", Action = "Thêm công nghệ", EntityType = "CommonTechnology", EntityId = newTech.TechId, OldValue = "", NewValue = techName.Trim(), CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thêm mới thành công công nghệ: {techName}!";
            
            return RedirectToAction("Index");
        }

        // 5. Action hiển thị form Edit
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var tech = await _techService.GetTechByIdAsync(id);
            if (tech == null) return NotFound();
            
            return View("~/Views/Moderator/TechStack/Edit.cshtml", tech);
        }

        // 4. Action xử lý POST khi lưu form Edit
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] string techName, [FromForm] bool isActive = false, [FromForm] int displayOrder = 1)
        {
            var tech = await _techService.GetTechByIdAsync(id);
            if (tech == null) return NotFound();

            if (string.IsNullOrWhiteSpace(techName))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên công nghệ.";
                return View("~/Views/Moderator/TechStack/Edit.cshtml", tech);
            }

            // Logic tương quan với BR-PRF-02: Ngăn chặn sửa tên công nghệ bị trùng với một công nghệ khác đang tồn tại.
            var allTechs = await _techService.GetAllTechsAsync();
            if (allTechs.Any(t => t.TechId != id && t.TechName.Equals(techName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.ErrorMessage = $"Lỗi: Công nghệ '{techName}' đã được sử dụng bởi một mục khác.";
                return View("~/Views/Moderator/TechStack/Edit.cshtml", tech);
            }

            tech.TechName = techName.Trim();
            tech.IsActive = isActive;
            tech.DisplayOrder = displayOrder;
            await _techService.UpdateTechAsync(tech); // Gọi Service xử lý lưu vào DB thông qua Repository

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                UserId = modId, UserType = "Moderator", Action = "Sửa công nghệ", EntityType = "CommonTechnology", EntityId = id, OldValue = "Tồn tại", NewValue = techName.Trim(), CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cập nhật thành công công nghệ #{id}!";
            
            return RedirectToAction("Index");
        }

        // 5. Action xử lý bật/tắt nhanh trạng thái (Fetch API)
        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            // Logic tương quan với BR-MOD-03: Sử dụng cờ IsActive để vô hiệu hóa/kích hoạt (soft-delete) công nghệ thay vì hard-delete để không làm hỏng dữ liệu liên kết.
            var success = await _techService.ToggleStatusAsync(id);
            if (!success) return NotFound();

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                UserId = modId, UserType = "Moderator", Action = "Đổi trạng thái công nghệ", EntityType = "CommonTechnology", EntityId = id, OldValue = "", NewValue = "Đã đổi", CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = $"Cập nhật trạng thái thành công công nghệ #{id}!" });
        }

        [HttpPost("update-order/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromForm] int displayOrder)
        {
            try {
                var tech = await _techService.GetTechByIdAsync(id);
                if (tech == null) return NotFound();

                int oldOrder = tech.DisplayOrder;
                tech.DisplayOrder = displayOrder;
                await _techService.UpdateTechAsync(tech);

                var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(modIdClaim, out int modId);
                _db.AuditLogs.Add(new DevHub.Models.AuditLog {
                    UserId = modId, UserType = "Moderator", Action = "Cập nhật thứ tự công nghệ", EntityType = "CommonTechnology", EntityId = id, OldValue = oldOrder.ToString(), NewValue = displayOrder.ToString(), CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = $"Cập nhật thứ tự thành công công nghệ #{id}!" });
            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

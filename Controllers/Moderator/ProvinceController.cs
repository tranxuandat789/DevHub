//KienHM-25/7/2026

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Services.Interfaces;
using DevHub.Models;

namespace DevHub.Controllers.Moderator
{
    [Route("ModeratorProvince")]
    [Authorize(Roles = "Moderator")]
    public class ProvinceController : Controller
    {
        private readonly IProvinceService _provinceService;
        private readonly DevHub.Data.ItrecruitmentDbContext _db;

        public ProvinceController(IProvinceService provinceService, DevHub.Data.ItrecruitmentDbContext db)
        {
            _provinceService = provinceService;
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string keyword = "", string status = "", string region = "", string sortOrder = "asc", int page = 1)
        {
            var provinces = await _provinceService.GetAllProvincesAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                provinces = provinces.Where(p => p.ProvinceName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool isActive = status == "1";
                provinces = provinces.Where(p => p.IsActive == isActive).ToList();
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                if (region == "North") provinces = provinces.Where(p => p.Region == "North").ToList();
                else if (region == "Central") provinces = provinces.Where(p => p.Region == "Central").ToList();
                else if (region == "South") provinces = provinces.Where(p => p.Region == "South").ToList();
            }

            if (sortOrder == "desc")
            {
                provinces = provinces
                    .OrderByDescending(p => p.DisplayOrder)
                    .ThenBy(p => p.ProvinceName)
                    .ToList();
            }
            else
            {
                provinces = provinces
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.ProvinceName)
                    .ToList();
            }

            int pageSize = 10;
            int totalItems = provinces.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedProvinces = provinces.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewData["Keyword"] = keyword;
            ViewData["Status"] = status;
            ViewData["Region"] = region;
            ViewData["SortOrder"] = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Moderator/Province/Index.cshtml", pagedProvinces);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Moderator/Province/Create.cshtml");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] string provinceName, [FromForm] string region, [FromForm] int? displayOrder)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên tỉnh/thành phố.";
                return View("~/Views/Moderator/Province/Create.cshtml");
            }

            // Logic tương quan với nguyên tắc của BR-PRF-02 (và tính nhất quán DB): Ngăn chặn tạo trùng lặp tên Tỉnh/Thành phố trong hệ thống (để bảo vệ tính toàn vẹn dữ liệu).
            var allProvinces = await _provinceService.GetAllProvincesAsync();
            if (allProvinces.Any(p => p.ProvinceName.Equals(provinceName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.ErrorMessage = $"Lỗi: Tỉnh/Thành phố '{provinceName}' đã tồn tại trong hệ thống.";
                return View("~/Views/Moderator/Province/Create.cshtml");
            }

            var newProvince = new Province
            {
                ProvinceName = provinceName.Trim(),
                Region = string.IsNullOrWhiteSpace(region) ? null : region,
                IsActive = true,
                DisplayOrder = displayOrder ?? 999
            };
            
            await _provinceService.AddProvinceAsync(newProvince);

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            _db.AuditLogs.Add(new AuditLog {
                UserId = modId, UserType = "Moderator", Action = "Thêm tỉnh/thành phố", EntityType = "Province", EntityId = newProvince.ProvinceId, OldValue = "", NewValue = provinceName.Trim(), CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thêm mới thành công tỉnh/thành phố: {provinceName}!";
            
            return RedirectToAction("Index");
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            // Logic tương quan với BR-MOD-03: Sử dụng cơ chế soft-delete (đổi trạng thái thay vì xóa cứng) để vô hiệu hóa tỉnh/thành phố nhằm bảo toàn dữ liệu của các Job Post đã chọn địa điểm này.
            var success = await _provinceService.ToggleStatusAsync(id);
            if (!success) return NotFound();

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            _db.AuditLogs.Add(new AuditLog {
                UserId = modId, UserType = "Moderator", Action = "Đổi trạng thái tỉnh/thành phố", EntityType = "Province", EntityId = id, OldValue = "", NewValue = "Đã đổi", CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = $"Cập nhật trạng thái thành công tỉnh/thành phố #{id}!" });
        }
        [HttpPost("update-order/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromForm] int displayOrder)
        {
            try {
                System.IO.File.AppendAllText("devhub_debug.log", $"UpdateOrder called for id={id}, displayOrder={displayOrder}\n");
                var province = await _provinceService.GetProvinceByIdAsync(id);
                if (province == null) {
                    System.IO.File.AppendAllText("devhub_debug.log", $"Province {id} not found\n");
                    return NotFound();
                }

                int oldOrder = province.DisplayOrder;
                province.DisplayOrder = displayOrder;
                await _provinceService.UpdateProvinceAsync(province);
                System.IO.File.AppendAllText("devhub_debug.log", $"Province updated\n");

                var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(modIdClaim, out int modId);
                _db.AuditLogs.Add(new AuditLog {
                    UserId = modId, UserType = "Moderator", Action = "Cập nhật thứ tự tỉnh/thành phố", EntityType = "Province", EntityId = id, OldValue = oldOrder.ToString(), NewValue = displayOrder.ToString(), CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
                System.IO.File.AppendAllText("devhub_debug.log", $"Audit log saved\n");

                return Json(new { success = true, message = $"Cập nhật thứ tự thành công tỉnh/thành phố #{id}!" });
            } catch (Exception ex) {
                System.IO.File.AppendAllText("devhub_debug.log", $"ERROR: {ex}\n");
                throw;
            }
        }
    }
}

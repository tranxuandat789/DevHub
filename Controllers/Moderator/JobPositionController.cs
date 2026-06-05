using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("ModeratorPosition")]
    [Authorize(Roles = "Moderator")]
    public class JobPositionController : Controller
    {
        private readonly DevHub.Services.Interfaces.ICommonJobPositionService _positionService;

        public JobPositionController(DevHub.Services.Interfaces.ICommonJobPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string keyword = "", string status = "")
        {
            var positions = await _positionService.GetAllPositionsAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                positions = positions.Where(p => p.PositionName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool isActive = status == "1";
                positions = positions.Where(p => (p.IsActive ?? true) == isActive).ToList();
            }

            ViewData["Keyword"] = keyword;
            ViewData["Status"] = status;

            return View("~/Views/Moderator/JobPosition/Index.cshtml", positions);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Moderator/JobPosition/Create.cshtml");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] string positionName, [FromForm] bool isActive = false)
        {
            if (!string.IsNullOrWhiteSpace(positionName))
            {
                var newPos = new DevHub.Models.CommonJobPosition
                {
                    PositionName = positionName.Trim(),
                    IsActive = isActive
                };
                await _positionService.AddPositionAsync(newPos);
                TempData["SuccessMessage"] = $"Thêm thành công vị trí: {positionName}!";
            }
            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var pos = await _positionService.GetPositionByIdAsync(id);
            if (pos == null) return NotFound();
            
            return View("~/Views/Moderator/JobPosition/Edit.cshtml", pos);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] string positionName, [FromForm] bool isActive = false)
        {
            var pos = await _positionService.GetPositionByIdAsync(id);
            if (pos == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(positionName))
            {
                pos.PositionName = positionName.Trim();
                pos.IsActive = isActive;
                await _positionService.UpdatePositionAsync(pos);
                TempData["SuccessMessage"] = $"Cập nhật thành công vị trí #{id}!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _positionService.ToggleStatusAsync(id);
            if (!success) return NotFound();

            return Json(new { success = true });
        }
    }
}

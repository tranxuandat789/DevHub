using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DevHub.Models;
using DevHub.Services.Interfaces;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/job-positions")]
    [Authorize(Roles = "Moderator,Admin")]
    public class JobPositionController : Controller
    {
        private readonly ICommonJobPositionService _positionService;

        public JobPositionController(ICommonJobPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("")]
        [HttpGet("/ModeratorPosition")]
        public async Task<IActionResult> Index(string search, string status)
        {
            var positions = await _positionService.GetAllPositionsAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                positions = positions.Where(p => p.PositionName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool isActive = status == "1";
                positions = positions.Where(p => (p.IsActive ?? true) == isActive).ToList();
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View("~/Views/Moderator/JobPosition/Index.cshtml", positions);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var position = await _positionService.GetPositionByIdAsync(id);
            if (position == null) return NotFound();
            return View("~/Views/Moderator/JobPosition/Edit.cshtml", position);
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] string positionName, [FromForm] bool isActive = false)
        {
            var position = await _positionService.GetPositionByIdAsync(id);
            if (position == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(positionName))
            {
                position.PositionName = positionName.Trim();
                position.IsActive = isActive;
                await _positionService.UpdatePositionAsync(position);
                TempData["SuccessMessage"] = $"Cập nhật thành công vị trí #{id}!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _positionService.ToggleStatusAsync(id);
            return Ok();
        }
    }
}

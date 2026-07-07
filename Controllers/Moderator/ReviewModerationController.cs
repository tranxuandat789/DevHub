using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/reviews")]
    [Authorize(Roles = "Moderator,MODERATOR")]
    [TypeFilter(typeof(DevHub.Filters.ModeratorTaskTypeAttribute), Arguments = new object[] { "REVIEW" })]
    public class ReviewModerationController : Controller
    {
        private readonly IReviewCompanyService _reviewService;

        public ReviewModerationController(IReviewCompanyService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? statusFilter, int page = 1)
        {
            int pageSize = 10;
            var (items, totalCount) = await _reviewService.GetPagedForModeratorAsync(statusFilter, page, pageSize);

            var model = new ReviewModerationViewModel
            {
                Reviews = items,
                StatusFilter = statusFilter,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                FromItem = (page - 1) * pageSize + 1,
                ToItem = Math.Min(page * pageSize, totalCount)
            };

            return View("~/Views/Moderator/ReviewApproval/Index.cshtml", model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View("~/Views/Moderator/ReviewApproval/Detail.cshtml", review);
        }

        [HttpPost("approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var moderatorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _reviewService.ApproveReviewAsync(id, moderatorId);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã duyệt đánh giá công ty thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi duyệt đánh giá.";
            }

            return RedirectToAction("Detail", new { id });
        }

        [HttpPost("reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập lý do từ chối.";
                return RedirectToAction("Detail", new { id });
            }

            var moderatorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _reviewService.RejectReviewAsync(id, moderatorId, rejectionReason);

            if (success)
            {
                TempData["SuccessMessage"] = "Đã từ chối đánh giá công ty.";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi từ chối đánh giá.";
            }

            return RedirectToAction("Detail", new { id });
        }
    }
}

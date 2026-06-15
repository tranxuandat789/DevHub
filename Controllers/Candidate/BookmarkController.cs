using DevHub.Services.Interfaces;
using DevHub.ViewModels.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/bookmarks")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class BookmarkController : Controller
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            string? workingModel, string? level, string? location, int page = 1)
        {
            const int pageSize = 3;
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Lấy trang hiện tại (đã filter)
            var (items, totalCount) = await _bookmarkService.GetPagedAsync(
                candidateId, page, pageSize, workingModel, level, location);

            // Lấy tất cả bookmark để build filter options (không phân trang)
            var allBookmarks = await _bookmarkService.GetByCandidateAsync(candidateId);

            var pageModel = new BookmarkPageViewModel
            {
                Jobs = items.Select(b => new BookmarkViewModel
                {
                    BookmarkId = b.BookmarkId,
                    JobId = b.JobId,
                    Title = b.Job?.Title ?? "",
                    CompanyName = b.Job?.Recruiter?.CompanyName ?? "",
                    CompanyLogoUrl = b.Job?.Recruiter?.CompanyLogoUrl,
                    Location = b.Job?.Location,
                    WorkingModel = b.Job?.WorkingModel,
                    ExperienceLevel = b.Job?.ExperienceLevel,
                    SalaryMin = b.Job?.SalaryMin,
                    SalaryMax = b.Job?.SalaryMax,
                    SavedAt = b.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                WorkingModelOptions = allBookmarks
                    .Select(b => b.Job?.WorkingModel).Where(v => !string.IsNullOrEmpty(v))
                    .Distinct().Order().ToList()!,
                ExperienceLevelOptions = allBookmarks
                    .Select(b => b.Job?.ExperienceLevel).Where(v => !string.IsNullOrEmpty(v))
                    .Distinct().Order().ToList()!,
                LocationOptions = allBookmarks
                    .Select(b => b.Job?.Location).Where(v => !string.IsNullOrEmpty(v))
                    .Distinct().Order().ToList()!,
                FilterWorkingModel = workingModel,
                FilterLevel = level,
                FilterLocation = location
            };

            ViewData["Title"] = "Việc làm đã lưu";
            ViewData["IsCandidate"] = "true";
            ViewData["ActiveMenu"] = "SavedJobs";
            return View("~/Views/Candidate/Bookmark/Index.cshtml", pageModel);
        }


        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle(int jobId)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isBookmarked = await _bookmarkService.ToggleAsync(candidateId, jobId);
            return Json(new { success = true, isBookmarked });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove(int jobId)
        {
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _bookmarkService.ToggleAsync(candidateId, jobId);
            return RedirectToAction("Index");
        }
    }
}
// Author: DatTX - Public job search controller
using System.Security.Claims;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Candidate;

public class JobsController : Controller
{
    private readonly IJobSearchService _jobSearchService;
    private readonly IBookmarkService _bookmarkService;
    private readonly IApplicationService _applicationService;

    public JobsController(IJobSearchService jobSearchService, IBookmarkService bookmarkService, IApplicationService applicationService)
    {
        _jobSearchService = jobSearchService;
        _bookmarkService = bookmarkService;
        _applicationService = applicationService;
    }

    /// GET /Jobs — Job search page. 
    /// Receives filter from query string, returns paginated job list.
    public async Task<IActionResult> Index([FromQuery] JobSearchFilterViewModel filter)
    {
        var model = await _jobSearchService.SearchJobsAsync(filter);

        // Load bookmark IDs nếu ứng viên đã đăng nhập
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("CANDIDATE") || User.IsInRole("Candidate"))
        {
            var candidateIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(candidateIdStr, out int candidateId))
                model.BookmarkedJobIds = await _bookmarkService.GetBookmarkedJobIdsAsync(candidateId);
        }

        return View("~/Views/Candidate/Job/Index.cshtml", model);
    }

    /// GET /Jobs/Details/{id} — Job details page.
    /// Returns 404 if job does not exist or is not APPROVED.
    [HttpGet("Job/Detail/{id}")]
    [HttpGet("Jobs/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var model = await _jobSearchService.GetJobDetailAsync(id);

        if (model is null)
            return NotFound();

        if (User.Identity?.IsAuthenticated == true && (User.IsInRole("CANDIDATE") || User.IsInRole("Candidate")))
        {
            var candidateIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(candidateIdStr, out int candidateId))
            {
                var bookmarkedJobIds = await _bookmarkService.GetBookmarkedJobIdsAsync(candidateId);
                model.IsBookmarked = bookmarkedJobIds.Contains(id);

                if (Request.Query.ContainsKey("fromApplied"))
                {
                    var appStatus = await _applicationService.GetApplicationStatusAsync(candidateId, id);
                    if (appStatus.HasValue)
                    {
                        ViewBag.AppStatus = appStatus.Value.Status;
                        ViewBag.AppliedAt = appStatus.Value.AppliedAt;
                        ViewBag.Interviews = appStatus.Value.Interviews;
                    }
                }
            }
        }

        return View("~/Views/Candidate/Job/Details.cshtml", model);
    }

    /// GET /Job/Apply/{id}
    /// Helper redirection for job applications from outside the details view.
    [HttpGet("Job/Apply/{id}")]
    public IActionResult ApplyJobRedirect(int id)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Details", "Jobs", new { id = id, apply = true });
        }
        else
        {
            return Redirect($"/Account/Login?returnUrl={Url.Content($"/Job/Apply/{id}")}");
        }
    }

    /// GET /Jobs/NavData — JSON data cho mega menu trong header.
    /// Trả về top 20 kỹ năng, thành phố, công ty có nhiều job nhất.
    [HttpGet]
    public async Task<IActionResult> NavData()
    {
        var data = await _jobSearchService.GetNavMenuDataAsync();
        return Json(data);
    }

    /// GET /Jobs/MakeExpired — Temporary test endpoint to make the first 2 jobs expired
    [HttpGet("Jobs/MakeExpired")]
    public async Task<IActionResult> MakeExpired([FromServices] DevHub.Data.ItrecruitmentDbContext context)
    {
        var jobs = context.JobPosts.Where(j => j.Status == "APPROVED").Take(2).ToList();
        foreach (var job in jobs)
        {
            job.Deadline = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));
        }
        await context.SaveChangesAsync();
        return Content($"Đã cập nhật {jobs.Count} công việc (ID: {string.Join(", ", jobs.Select(j => j.JobId))}) thành quá hạn (cách đây 5 ngày).");
    }

    /// GET /Jobs/ApplyInfo — Get candidate info for apply modal
    [HttpGet]
    [Authorize(Roles = "CANDIDATE")]
    public async Task<IActionResult> ApplyInfo()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int candidateId))
            return Unauthorized();

        var applyInfo = await _applicationService.GetApplyInfoAsync(candidateId);
        if (applyInfo == null)
            return NotFound();

        return Json(applyInfo);
    }

    /// POST /Jobs/Apply — Submit job application
    [HttpPost]
    [Authorize(Roles = "CANDIDATE")]
    public async Task<IActionResult> Apply([FromBody] SubmitApplyViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int candidateId))
            return Unauthorized();

        var (success, message) = await _applicationService.ApplyForJobAsync(candidateId, model);

        return Json(new { success, message });
    }
}



// Author: [your-name] - Public job search controller
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevHub.Controllers;

public class JobsController : Controller
{
    private readonly IJobSearchService _jobSearchService;
    private readonly IBookmarkService _bookmarkService;

    public JobsController(IJobSearchService jobSearchService, IBookmarkService bookmarkService)
    {
        _jobSearchService = jobSearchService;
        _bookmarkService = bookmarkService;
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
    public async Task<IActionResult> Details(int id)
    {
        var model = await _jobSearchService.GetJobDetailAsync(id);

        if (model is null)
            return NotFound();

        return View("~/Views/Candidate/Job/Details.cshtml", model);
    }

    /// GET /Jobs/NavData — JSON data cho mega menu trong header.
    /// Trả về top 20 kỹ năng, thành phố, công ty có nhiều job nhất.
    [HttpGet]
    public async Task<IActionResult> NavData()
    {
        var data = await _jobSearchService.GetNavMenuDataAsync();
        return Json(data);
    }
}


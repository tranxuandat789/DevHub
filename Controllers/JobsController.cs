// Author: [your-name] - Public job search controller
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

public class JobsController : Controller
{
    private readonly IJobSearchService _jobSearchService;

    public JobsController(IJobSearchService jobSearchService)
    {
        _jobSearchService = jobSearchService;
    }

    /// GET /Jobs — Job search page. 
    /// Receives filter from query string, returns paginated job list.
    public async Task<IActionResult> Index([FromQuery] JobSearchFilterViewModel filter)
    {
        var model = await _jobSearchService.SearchJobsAsync(filter);
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
}

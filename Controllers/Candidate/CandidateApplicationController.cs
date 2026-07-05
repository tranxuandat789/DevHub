using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/applications")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;

        public CandidateApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            string? keyword, string? timeRange, string? status, int page = 1)
        {
            const int pageSize = 3;
            var candidateId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _applicationService.GetPagedAppliedAsync(
                candidateId, page, pageSize, keyword, timeRange, status);
                
            ViewData["Title"] = "Việc làm đã ứng tuyển";
            ViewData["IsCandidate"] = "true";
            ViewData["ActiveMenu"] = "AppliedJobs";
            return View("~/Views/Candidate/CandidateApplication/AppliedJobs.cshtml", model);
        }
    }
}
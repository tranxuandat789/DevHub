using DevHub.Data;
using DevHub.Models;
using DevHub.ViewModels.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DevHub.Services.Interfaces;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/interviews")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateInterviewController : Controller
    {
        private readonly ItrecruitmentDbContext _context;
        private readonly IInterviewService _interviewService;

        public CandidateInterviewController(ItrecruitmentDbContext context, IInterviewService interviewService)
        {
            _context = context;
            _interviewService = interviewService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int? jobId, [FromQuery] string tab = "all", [FromQuery] int page = 1)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int candidateId)) return RedirectToAction("Login", "Auth");

            await _interviewService.SyncInterviewStatusesAsync();

            var viewModel = new CandidateInterviewViewModel
            {
                SelectedJobId = jobId,
                ActiveTab = tab,
                CurrentPage = page
            };

            // Get Job Posts for filter (jobs that the candidate has applied to and got an interview)
            viewModel.JobPosts = await _context.Interviews
                .Include(i => i.Application).ThenInclude(a => a.Job)
                .Where(i => i.CandidateId == candidateId)
                .Select(i => i.Application.Job)
                .Distinct()
                .ToListAsync();

            // Base query for interviews
            var query = _context.Interviews
                .Include(i => i.Application).ThenInclude(a => a.Job).ThenInclude(j => j.Company)
                .Where(i => i.CandidateId == candidateId)
                .AsQueryable();

            if (jobId.HasValue)
            {
                query = query.Where(i => i.Application.JobId == jobId.Value);
            }

            var now = DateTime.Now;

            // Filter out "completed_pending" equivalent (scheduled but time passed)
            query = query.Where(i => !(i.Status == "scheduled" && i.ScheduledTime < now));

            // Get all raw matches to count tabs
            var allInterviews = await query.ToListAsync();

            viewModel.ScheduledCount = allInterviews.Count(i => i.Status == "scheduled");
            viewModel.PassedCount = allInterviews.Count(i => i.Status == "passed");
            viewModel.RejectedCount = allInterviews.Count(i => i.Status == "rejected");
            viewModel.CancelledCount = allInterviews.Count(i => i.Status == "cancelled");

            if (tab == "scheduled")
            {
                query = query.Where(i => i.Status == "scheduled" && i.ScheduledTime >= now);
            }
            else if (tab == "passed")
            {
                query = query.Where(i => i.Status == "passed");
            }
            else if (tab == "rejected")
            {
                query = query.Where(i => i.Status == "rejected");
            }
            else if (tab == "cancelled")
            {
                query = query.Where(i => i.Status == "cancelled");
            }
            // tab == "all": không filter gì cả

            // Pagination
            int pageSize = 6;
            viewModel.TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
            if (viewModel.TotalPages == 0) viewModel.TotalPages = 1;
            
            viewModel.Interviews = await query
                .OrderByDescending(i => i.ScheduledTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // (Mapping to completed_pending removed)

            return View("~/Views/Candidate/CandidateInterview/Index.cshtml", viewModel);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int candidateId)) return RedirectToAction("Login", "Auth");

            var interview = await _context.Interviews
                .Include(i => i.Application).ThenInclude(a => a.Job).ThenInclude(j => j.Company)
                .FirstOrDefaultAsync(i => i.InterviewId == id && i.CandidateId == candidateId);

            if (interview == null) return NotFound();

            // Query recruiter info separately
            var recruiter = await _context.Recruiters
                .Include(r => r.RecruiterNavigation)
                .FirstOrDefaultAsync(r => r.RecruiterId == interview.RecruiterId);
            ViewBag.Recruiter = recruiter;

            return View("~/Views/Candidate/CandidateInterview/Details.cshtml", interview);
        }
    }
}
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
        public async Task<IActionResult> Index([FromQuery] int? jobId, [FromQuery] string tab = "scheduled", [FromQuery] int page = 1)
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

            // Get all raw matches to count tabs
            var allInterviews = await query.ToListAsync();

            viewModel.ScheduledCount = allInterviews.Count(i => i.Status == "scheduled" && i.ScheduledTime >= now);
            viewModel.CompletedCount = allInterviews.Count(i => i.Status == "completed_pending" || (i.Status == "scheduled" && i.ScheduledTime < now));
            viewModel.PassedCount = allInterviews.Count(i => i.Status == "passed");
            viewModel.RejectedCount = allInterviews.Count(i => i.Status == "rejected");
            viewModel.CancelledCount = allInterviews.Count(i => i.Status == "cancelled");

            // Apply Tab Filter
            if (tab == "scheduled")
            {
                query = query.Where(i => i.Status == "scheduled" && i.ScheduledTime >= now);
            }
            else if (tab == "completed_pending")
            {
                query = query.Where(i => i.Status == "completed_pending" || (i.Status == "scheduled" && i.ScheduledTime < now));
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

            // Pagination
            int pageSize = 6;
            viewModel.TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
            if (viewModel.TotalPages == 0) viewModel.TotalPages = 1;
            
            viewModel.Interviews = await query
                .OrderByDescending(i => i.ScheduledTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map virtual status for UI display
            foreach (var interview in viewModel.Interviews)
            {
                if (interview.Status == "scheduled" && interview.ScheduledTime < now)
                {
                    interview.Status = "completed_pending";
                }
            }

            return View("~/Views/Candidate/CandidateInterview/Index.cshtml", viewModel);
        }
    }
}
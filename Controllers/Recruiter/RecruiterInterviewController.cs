using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/[controller]")]
    [Authorize(Roles = "RECRUITER")]
    public class RecruiterInterviewController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IInterviewService _interviewService;
        private readonly ItrecruitmentDbContext _context;

        public RecruiterInterviewController(IAuthService authService, IInterviewService interviewService, ItrecruitmentDbContext context)
        {
            _authService = authService;
            _interviewService = interviewService;
            _context = context;
        }

        private async Task<int?> GetRecruiterIdAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            return dbUser?.Recruiter?.RecruiterId;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int? jobId, [FromQuery] string tab = "scheduled", [FromQuery] string search = "", [FromQuery] int page = 1)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return RedirectToAction("Login", "Auth");

            var companyId = await _context.Recruiters
                .Where(r => r.RecruiterId == recruiterId)
                .Select(r => r.CompanyId)
                .FirstOrDefaultAsync();

            await _interviewService.SyncInterviewStatusesAsync();

            var viewModel = new RecruiterInterviewViewModel
            {
                SelectedJobId = jobId,
                ActiveTab = tab,
                SearchTerm = search,
                CurrentPage = page
            };

            viewModel.JobPosts = await _context.JobPosts
                .Where(j => j.CompanyId == companyId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            // Base query for interviews
            var query = _context.Interviews
                .Include(i => i.Application).ThenInclude(a => a.Job)
                .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
                .Where(i => i.Application.Job.CompanyId == companyId)
                .AsQueryable();

            if (jobId.HasValue)
            {
                query = query.Where(i => i.Application.JobId == jobId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i => i.Candidate.FullName.Contains(search) || i.Candidate.CandidateNavigation.Email.Contains(search));
            }

            // Status Logic: Move scheduled past now to completed_pending virtually for counts/display
            var now = DateTime.Now;

            // Get all raw matches to count tabs
            var allInterviews = await query.ToListAsync();

            viewModel.ScheduledCount = allInterviews.Count(i => i.Status == "scheduled" && i.ScheduledTime >= now);
            // Include virtually passed or explicitly marked completed_pending
            viewModel.CompletedCount = allInterviews.Count(i => i.Status == "completed_pending" || (i.Status == "scheduled" && i.ScheduledTime < now));
            viewModel.PassedCount = allInterviews.Count(i => i.Status == "passed");
            viewModel.RejectedCount = allInterviews.Count(i => i.Status == "rejected");

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

            return View("~/Views/Recruiter/RecruiterInterview/Index.cshtml", viewModel);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return RedirectToAction("Login", "Auth");

            var companyId = await _context.Recruiters
                .Where(r => r.RecruiterId == recruiterId)
                .Select(r => r.CompanyId)
                .FirstOrDefaultAsync();

            ViewBag.JobPosts = await _context.JobPosts
                .Where(j => j.CompanyId == companyId && (j.Status == "ACTIVE" || j.Status == "APPROVED" || j.Status == "Active"))
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View("~/Views/Recruiter/RecruiterInterview/Create.cshtml");
        }

        [HttpGet("GetApplicationsByJob")]
        public async Task<IActionResult> GetApplicationsByJob([FromQuery] int jobId)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized();

            var companyId = await _context.Recruiters
                .Where(r => r.RecruiterId == recruiterId)
                .Select(r => r.CompanyId)
                .FirstOrDefaultAsync();

            var hasAccess = await _context.JobPosts.AnyAsync(j => j.JobId == jobId && j.CompanyId == companyId);
            if (!hasAccess) return BadRequest("Invalid Job");

            var applications = await _context.Applications
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateNavigation)
                .Where(a => a.JobId == jobId && a.Status != "REJECTED" && a.Status != "CANCELLED" && a.Status != "HIRED" && a.Status != "FAILED")
                // exclude candidates that already have any non-cancelled interview
                .Where(a => !a.Interviews.Any(i => i.Status != "cancelled"))
                .Select(a => new {
                    id = a.ApplicationId,
                    name = a.Candidate.FullName + " (" + a.Candidate.CandidateNavigation.Email + ")"
                })
                .ToListAsync();

            return Json(applications);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return RedirectToAction("Login", "Auth");

            var companyId = await _context.Recruiters
                .Where(r => r.RecruiterId == recruiterId)
                .Select(r => r.CompanyId)
                .FirstOrDefaultAsync();

            var interview = await _context.Interviews
                .Include(i => i.Application).ThenInclude(a => a.Job)
                .Include(i => i.Candidate)
                .FirstOrDefaultAsync(i => i.InterviewId == id && i.Application.Job.CompanyId == companyId);

            if (interview == null) return NotFound();

            return View("~/Views/Recruiter/RecruiterInterview/Edit.cshtml", interview);
        }
        
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return RedirectToAction("Login", "Auth");

            var companyId = await _context.Recruiters
                .Where(r => r.RecruiterId == recruiterId)
                .Select(r => r.CompanyId)
                .FirstOrDefaultAsync();

            var interview = await _context.Interviews
                .Include(i => i.Application).ThenInclude(a => a.Job)
                .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
                .FirstOrDefaultAsync(i => i.InterviewId == id && i.Application.Job.CompanyId == companyId);

            if (interview == null) return NotFound();

            return View("~/Views/Recruiter/RecruiterInterview/Details.cshtml", interview);
        }

        public class InterviewDto
        {
            public int ApplicationId { get; set; }
            public DateTime InterviewDate { get; set; }
            public int DurationMinutes { get; set; }
            public string InterviewType { get; set; } = "";
            public string LocationOrLink { get; set; } = "";
            public string? Notes { get; set; }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost([FromBody] InterviewDto model)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

            if (model.ApplicationId <= 0 || string.IsNullOrEmpty(model.InterviewType) || string.IsNullOrEmpty(model.LocationOrLink))
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });

            if (model.InterviewDate <= DateTime.Now)
                return BadRequest(new { success = false, message = "Thời gian phỏng vấn phải lớn hơn thời gian hiện tại." });

            try
            {
                var interview = await _interviewService.CreateInterviewAsync(recruiterId.Value, model.ApplicationId, model.InterviewDate, model.InterviewType, model.LocationOrLink, model.Notes);
                return Ok(new { success = true, message = "Tạo lịch phỏng vấn thành công.", interviewId = interview.InterviewId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditPost(int id, [FromBody] InterviewDto model)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

            if (string.IsNullOrEmpty(model.InterviewType) || string.IsNullOrEmpty(model.LocationOrLink))
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });

            if (model.InterviewDate <= DateTime.Now)
                return BadRequest(new { success = false, message = "Thời gian phỏng vấn phải lớn hơn thời gian hiện tại." });

            try
            {
                await _interviewService.UpdateInterviewAsync(recruiterId.Value, id, model.InterviewDate, model.InterviewType, model.LocationOrLink, model.Notes);
                return Ok(new { success = true, message = "Cập nhật lịch phỏng vấn thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Không có quyền." });

            var validStatuses = new[] { "passed", "rejected", "cancelled" };
            if (!validStatuses.Contains(status)) return BadRequest(new { success = false, message = "Trạng thái không hợp lệ." });

            var success = await _interviewService.UpdateStatusAsync(recruiterId.Value, id, status);
            if (!success) return BadRequest(new { success = false, message = "Cập nhật thất bại." });

            return Ok(new { success = true, message = "Cập nhật thành công." });
        }
    }
}

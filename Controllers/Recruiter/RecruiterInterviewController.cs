using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> Index([FromQuery] int? jobId, [FromQuery] string tab = "all", [FromQuery] string search = "", [FromQuery] int page = 1)
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

        
            var now = DateTime.Now;
            query = query.Where(i => !(i.Status == "completed_pending" || (i.Status == "scheduled" && i.ScheduledTime < now)));

            // Get all raw matches to count tabs
            var allInterviews = await query.ToListAsync();

            viewModel.AllCount = allInterviews.Count;
            viewModel.ScheduledCount = allInterviews.Count(i => i.Status == "scheduled" && i.ScheduledTime >= now);
            viewModel.CompletedCount = 0; // Đã loại bỏ
            viewModel.PassedCount = allInterviews.Count(i => i.Status == "passed");
            viewModel.RejectedCount = allInterviews.Count(i => i.Status == "rejected");
            viewModel.CancelledCount = allInterviews.Count(i => i.Status == "cancelled");

            // Apply Tab Filter
            if (tab == "all")
            {
                // No filter - return all
            }
            else if (tab == "scheduled")
            {
                query = query.Where(i => i.Status == "scheduled" && i.ScheduledTime >= now);
            }
            else if (tab == "completed_pending")
            {
                query = query.Where(i => false); // Đã loại bỏ
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
            int pageSize = 10;
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
                // exclude candidates that already have an active scheduled interview
                .Where(a => !a.Interviews.Any(i => i.Status == "scheduled"))
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
            [Required(ErrorMessage = "Vui lòng chọn ứng viên.")]
            [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn ứng viên hợp lệ.")]
            public int ApplicationId { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn thời gian phỏng vấn.")]
            public DateTime InterviewDate { get; set; }

            [Range(15, 480, ErrorMessage = "Thời lượng phải từ 15 đến 480 phút.")]
            public int DurationMinutes { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn hình thức phỏng vấn.")]
            public string InterviewType { get; set; } = "";

            [Required(ErrorMessage = "Vui lòng nhập link meeting hoặc địa điểm.")]
            public string LocationOrLink { get; set; } = "";

            public string? Notes { get; set; }

            public string? ReasonForChange { get; set; }
        }


        [HttpPost("Create")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreatePost([FromBody] InterviewDto model)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

            if (model == null)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ.", fields = new Dictionary<string, string>() });

            // Validate từng trường
            var fieldErrors = new Dictionary<string, string>();
            if (model.ApplicationId <= 0)
                fieldErrors["applicationId"] = "Vui lòng chọn ứng viên.";
            if (model.InterviewDate == default || model.InterviewDate <= DateTime.Now)
                fieldErrors["interviewDate"] = "Vui lòng chọn thời gian trong tương lai.";
            if (string.IsNullOrWhiteSpace(model.InterviewType))
                fieldErrors["interviewType"] = "Vui lòng chọn hình thức phỏng vấn.";
            if (string.IsNullOrWhiteSpace(model.LocationOrLink))
                fieldErrors["locationOrLink"] = "Vui lòng nhập link meeting hoặc địa điểm.";

            if (fieldErrors.Any())
                return BadRequest(new { success = false, message = "Vui lòng kiểm tra lại các trường bắt buộc.", fields = fieldErrors });

            try
            {
                var interview = await _interviewService.CreateInterviewAsync(recruiterId.Value, model.ApplicationId, model.InterviewDate, model.InterviewType, model.LocationOrLink, model.Notes);
                return Ok(new { success = true, message = "Tạo lịch phỏng vấn thành công.", interviewId = interview.InterviewId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        [HttpPost("Edit/{id}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditPost(int id, [FromBody] InterviewDto model)
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });

            if (model == null || string.IsNullOrEmpty(model.InterviewType) || string.IsNullOrEmpty(model.LocationOrLink))
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ thông tin bắt buộc." });

            if (model.InterviewDate <= DateTime.Now)
                return BadRequest(new { success = false, message = "Thời gian phỏng vấn phải lớn hơn thời gian hiện tại." });

            try
            {
                await _interviewService.UpdateInterviewAsync(recruiterId.Value, id, model.InterviewDate, model.InterviewType, model.LocationOrLink, model.Notes, model.ReasonForChange);
                return Ok(new { success = true, message = "Cập nhật lịch phỏng vấn thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status, [FromQuery] string reason = "")
        {
            var recruiterId = await GetRecruiterIdAsync();
            if (recruiterId == null) return Unauthorized(new { success = false, message = "Không có quyền." });

            var validStatuses = new[] { "passed", "rejected", "cancelled" };
            if (!validStatuses.Contains(status)) return BadRequest(new { success = false, message = "Trạng thái không hợp lệ." });

            var success = await _interviewService.UpdateStatusAsync(recruiterId.Value, id, status, reason);
            if (!success) return BadRequest(new { success = false, message = "Cập nhật thất bại." });

            return Ok(new { success = true, message = "Cập nhật thành công." });
        }
    }
}

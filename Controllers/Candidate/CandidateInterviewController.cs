// =========================================================================
// Chức năng xem và phản hồi lịch phỏng vấn cho Ứng viên
// Author: PhongDH
// =========================================================================
using DevHub.Data;
using DevHub.Models;
using DevHub.ViewModels.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DevHub.Services.Interfaces;
using DevHub.Helpers;

namespace DevHub.Controllers.Candidate
{
    [Route("candidate/interviews")]
    [Authorize(Roles = "CANDIDATE,Candidate")]
    public class CandidateInterviewController : Controller
    {
        private readonly ItrecruitmentDbContext _context;
        private readonly IInterviewService _interviewService;
        private readonly EmailHelper _emailHelper;

        public CandidateInterviewController(ItrecruitmentDbContext context, IInterviewService interviewService, EmailHelper emailHelper)
        {
            _context = context;
            _interviewService = interviewService;
            _emailHelper = emailHelper;
        }

        [HttpGet]
        // Xem danh sách lịch phỏng vấn của ứng viên, lọc theo công việc và trạng thái
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

            // Filter out "completed_pending" equivalent (scheduled/confirmed but time passed)
            query = query.Where(i => !((i.Status == "scheduled" || i.Status == "confirmed") && i.ScheduledTime < now));

            // Get all raw matches to count tabs
            var allInterviews = await query.ToListAsync();

            viewModel.ScheduledCount = allInterviews.Count(i => i.Status == "scheduled" || i.Status == "confirmed");
            viewModel.PassedCount = allInterviews.Count(i => i.Status == "passed");
            viewModel.RejectedCount = allInterviews.Count(i => i.Status == "rejected");
            viewModel.CancelledCount = allInterviews.Count(i => i.Status == "cancelled");

            if (tab == "scheduled")
            {
                query = query.Where(i => (i.Status == "scheduled" || i.Status == "confirmed") && i.ScheduledTime >= now);
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

        [HttpPost("confirm/{id:int}")]
        [IgnoreAntiforgeryToken]
        // Xác nhận tham gia phỏng vấn và gửi thông báo, email cho nhà tuyển dụng
        public async Task<IActionResult> ConfirmAttendance(int id)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int candidateId)) return Unauthorized();

                var interview = await _context.Interviews
                    .Include(i => i.Application).ThenInclude(a => a.Job)
                    .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
                    .FirstOrDefaultAsync(i => i.InterviewId == id && i.CandidateId == candidateId);

                if (interview == null || interview.Status != "scheduled") return NotFound();

                interview.Status = "confirmed";
                interview.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                var recruiter = await _context.Recruiters
                    .Include(r => r.RecruiterNavigation)
                    .FirstOrDefaultAsync(r => r.RecruiterId == interview.RecruiterId);

                if (recruiter != null && recruiter.RecruiterNavigation != null)
                {
                    // In-app Notification
                    var n = new Notification
                    {
                        UserId = recruiter.RecruiterNavigation.UserId,
                        UserType = "RECRUITER",
                        Type = "INTERVIEW",
                        Title = "Ứng viên xác nhận tham gia phỏng vấn",
                        Message = $"Ứng viên {interview.Candidate?.FullName} đã xác nhận tham gia buổi phỏng vấn cho vị trí {interview.Application?.Job?.Title}.",
                        ReferenceType = "Interview",
                        ReferenceId = interview.InterviewId,
                        SeverityLevel = "info",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    if (n.Message.Length > 500)
                        n.Message = n.Message.Substring(0, 497) + "...";

                    _context.Notifications.Add(n);
                    await _context.SaveChangesAsync();

                    // Email Notification
                    var emailSubject = "Ứng viên xác nhận tham gia phỏng vấn";
                    var emailBody = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6; max-width: 600px; margin: 0 auto; color: #333;'>
                            <h2 style='color: #4f46e5;'>DevHub - Phản hồi từ ứng viên</h2>
                            <p>Chào {recruiter.FullName},</p>
                            <p>Ứng viên <strong>{interview.Candidate?.FullName}</strong> đã <strong>xác nhận tham gia</strong> buổi phỏng vấn cho vị trí <strong>{interview.Application?.Job?.Title}</strong>.</p>
                            <p><strong>Thời gian:</strong> {interview.ScheduledTime:dd/MM/yyyy HH:mm}</p>
                            <p>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</p>
                            <br/>
                            <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                        </div>";
                    try
                    {
                        await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailBody);
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Error sending email: {emailEx.Message}");
                        // Continue even if email fails
                    }
                }

                return Ok(new { success = true, message = "Đã xác nhận tham gia thành công!" });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : "";
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}. Inner: {inner}", stack = ex.StackTrace });
            }
        }

        [HttpPost("cancel/{id:int}")]
        [IgnoreAntiforgeryToken]
        // Từ chối/hủy tham gia phỏng vấn kèm lý do, gửi thông báo cho nhà tuyển dụng
        public async Task<IActionResult> CancelAttendance(int id, [FromForm] string reason)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int candidateId)) return Unauthorized();

                var interview = await _context.Interviews
                    .Include(i => i.Application).ThenInclude(a => a.Job)
                    .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
                    .FirstOrDefaultAsync(i => i.InterviewId == id && i.CandidateId == candidateId);

                if (interview == null || (interview.Status != "scheduled" && interview.Status != "confirmed")) return NotFound();

                // Update status to cancelled
                interview.Status = "cancelled";
                interview.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                var recruiter = await _context.Recruiters
                    .Include(r => r.RecruiterNavigation)
                    .FirstOrDefaultAsync(r => r.RecruiterId == interview.RecruiterId);

                if (recruiter != null && recruiter.RecruiterNavigation != null)
                {
                    var reasonText = string.IsNullOrWhiteSpace(reason) ? "Không có lý do cụ thể" : reason;
                    
                    // In-app Notification
                    var n = new Notification
                    {
                        UserId = recruiter.RecruiterNavigation.UserId,
                        UserType = "RECRUITER",
                        Type = "INTERVIEW",
                        Title = "Ứng viên không thể tham gia phỏng vấn",
                        Message = $"Ứng viên {interview.Candidate?.FullName} đã thông báo không thể tham gia buổi phỏng vấn cho vị trí {interview.Application?.Job?.Title}. Lý do: {reasonText}",
                        ReferenceType = "Interview",
                        ReferenceId = interview.InterviewId,
                        SeverityLevel = "warning",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    if (n.Message.Length > 500)
                        n.Message = n.Message.Substring(0, 497) + "...";

                    _context.Notifications.Add(n);
                    await _context.SaveChangesAsync();

                    // Email Notification
                    var emailSubject = "Ứng viên không thể tham gia phỏng vấn";
                    var emailBody = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6; max-width: 600px; margin: 0 auto; color: #333;'>
                            <h2 style='color: #e63946;'>DevHub - Phản hồi từ ứng viên</h2>
                            <p>Chào {recruiter.FullName},</p>
                            <p>Ứng viên <strong>{interview.Candidate?.FullName}</strong> đã thông báo <strong>không thể tham gia</strong> buổi phỏng vấn cho vị trí <strong>{interview.Application?.Job?.Title}</strong>.</p>
                            <p><strong>Lý do:</strong> {reasonText}</p>
                            <p>Lịch phỏng vấn đã được cập nhật thành trạng thái 'Đã hủy'. Vui lòng đăng nhập hệ thống để sắp xếp lại lịch nếu cần.</p>
                            <br/>
                            <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                        </div>";
                    try
                    {
                        await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailBody);
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Error sending email: {emailEx.Message}");
                    }
                }

                return Ok(new { success = true, message = "Đã thông báo cho nhà tuyển dụng thành công!" });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : "";
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}. Inner: {inner}", stack = ex.StackTrace });
            }
        }
    }
}
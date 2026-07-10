
//5/2/2026 
//file view duyet bai dang
//author: Hoang Minh Kien


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Moderator;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers.Moderator
{
    [Route("moderator/job-approvals")]
    [Authorize(Roles = "Moderator")]
    [TypeFilter(typeof(DevHub.Filters.ModeratorTaskTypeAttribute), Arguments = new object[] { "JOB_POST" })]
    public class JobApprovalController : Controller
    {
        private readonly IJobPostService _jobPostService;
        private readonly DevHub.Data.ItrecruitmentDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly DevHub.Helpers.EmailHelper _emailHelper;

        public JobApprovalController(IJobPostService jobPostService, DevHub.Data.ItrecruitmentDbContext db, INotificationService notificationService, DevHub.Helpers.EmailHelper emailHelper)
        {
            _jobPostService = jobPostService;
            _db = db;
            _notificationService = notificationService;
            _emailHelper = emailHelper;
        }

        // Action xử lý yêu cầu GET đến địa chỉ "moderator/job-approvals" để hiển thị danh sách bài đăng chờ duyệt
        [HttpGet("")]
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string? sortOrder, int page = 1)
        {
            // Validate để tránh lỗi SqlDateTime overflow (Năm < 1753) và chặn lọc tương lai
            bool hasInvalidDate = false;
            bool isFutureDate = false;
            var maxDate = DateTime.Now;

            if (fromDate.HasValue)
            {
                if (fromDate.Value.Year < 1753)
                {
                    fromDate = null;
                    hasInvalidDate = true;
                }
                else if (fromDate.Value > maxDate)
                {
                    fromDate = null;
                    isFutureDate = true;
                }
            }

            if (toDate.HasValue)
            {
                if (toDate.Value.Year < 1753)
                {
                    toDate = null;
                    hasInvalidDate = true;
                }
                else if (toDate.Value > maxDate)
                {
                    toDate = null;
                    isFutureDate = true;
                }
            }

            if (hasInvalidDate)
            {
                ViewBag.ErrorMessage = "Vui lòng nhập năm từ 1753 trở đi. Các ngày quá cũ đã bị bỏ qua.";
            }
            if (isFutureDate)
            {
                ViewBag.ErrorMessage = "Ngày lọc không được vượt quá thời gian hiện tại. Dữ liệu đã được tự động điều chỉnh.";
            }

            // 1. Phân trang ngay ở SQL (Skip/Take) — Service trả về (trang hiện tại, tổng số bài)
            // Extract current moderator ID
            int moderatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Fetch pending jobs
            const int pageSize = 10;
            var (pageItems, totalCount) = await _jobPostService.GetPendingJobsAsync(moderatorId, fromDate, toDate, sortOrder, page, pageSize);

            // 2. Tính số trang + clamp trang hiện tại cho UI (cùng cách clamp với repository)
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            page = Math.Min(Math.Max(1, page), totalPages);

            // 3. Khởi tạo ViewModel để truyền dữ liệu, tiêu chí lọc và thông tin phân trang ra View
            var viewModel = new JobApprovalViewModel
            {
                JobPosts = pageItems,
                FromDate = fromDate,
                ToDate = toDate,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                FromItem = totalCount == 0 ? 0 : (page - 1) * pageSize + 1,
                ToItem = Math.Min(page * pageSize, totalCount)
            };

            // 4. Trả về View giao diện tương ứng kèm theo dữ liệu (ViewModel)
            return View("~/Views/Moderator/JobApproval/Index.cshtml", viewModel);
        }

        // Action xử lý yêu cầu POST đến địa chỉ "moderator/job-approvals/approve/{id}" để phê duyệt bài đăng
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            // 1. Lấy thông tin ID của Moderator đang đăng nhập từ hệ thống Claims của User
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra: Nếu không tìm thấy ID hoặc không thể ép kiểu ID từ chuỗi sang số nguyên (int)
            // thì trả về lỗi 400 (Bad Request) báo phiên làm việc không hợp lệ
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            // 2. Gọi Service thực hiện nghiệp vụ phê duyệt bài đăng
            var success = await _jobPostService.ApproveJobAsync(id, moderatorId);

            if (!success) return BadRequest("Cannot approve this job.");

            // Lấy thông tin job và gửi thông báo cho recruiter
            var job = await _jobPostService.GetJobPostByIdAsync(id);
            if (job != null)
            {
                var recruiters = await _db.Recruiters
                    .Include(r => r.RecruiterNavigation) // Include UserAccount to get Email
                    .Where(r => r.CompanyId == job.CompanyId)
                    .ToListAsync();

                foreach (var recruiter in recruiters)
                {
                    try
                    {
                        // 1. Gửi thông báo In-App
                        await _notificationService.SendNotificationAsync(
                            userId: recruiter.RecruiterId,
                            userType: "RECRUITER",
                            title: "Bài đăng đã được duyệt!",
                            message: $"Tin tuyển dụng \"{job.Title}\" của bạn đã được kiểm duyệt viên phê duyệt và hiện đang hiển thị trên hệ thống.",
                            type: "JOB_POST",
                            severity: "success",
                            referenceId: id,
                            referenceType: "JobPost"
                        );

                        // 2. Gửi Email (nếu recruiter cho phép nhận thông báo qua email)
                        if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email))
                        {
                            if (recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                            {
                                var emailSubject = $"[DevHub] Bài đăng đã được duyệt: {job.Title}";
                                var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                    "Bài đăng đã được duyệt",
                                    $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                    $"<p>Tin tuyển dụng <strong>\"{job.Title}\"</strong> của bạn đã được hệ thống phê duyệt và hiện đang được hiển thị trực tuyến.</p>" +
                                    $"<p>Các ứng viên bây giờ đã có thể xem và ứng tuyển vào vị trí này.</p>" +
                                    $"<p>Cảm ơn bạn đã đồng hành cùng DevHub!</p>"
                                );
                                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                            }
                        }
                    }
                    catch { /* best-effort */ }
                }
            }

            var auditLog = new DevHub.Models.AuditLog
            {
                UserId = moderatorId,
                UserType = "Moderator",
                Action = "Duyệt bài đăng",
                EntityType = "JobPost",
                EntityId = id,
                OldValue = "Chờ duyệt",
                NewValue = "Đã duyệt",
                CreatedAt = DateTime.UtcNow
            };
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();
            
            return Ok(new { success = true });
        }

        // Action xử lý yêu cầu POST đến địa chỉ "moderator/job-approvals/reject/{id}" để từ chối bài đăng
        // Tham số "reason" (lý do từ chối) được lấy từ dữ liệu Form gửi lên ([FromForm])
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromForm] string reason)
        {
            // 1. Lấy thông tin ID của Moderator đang đăng nhập từ hệ thống Claims của User
            var moderatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra tính hợp lệ của phiên đăng nhập của Moderator
            if (string.IsNullOrEmpty(moderatorIdClaim) || !int.TryParse(moderatorIdClaim, out int moderatorId))
            {
                return BadRequest("Invalid moderator session.");
            }

            // 2. Gọi Service thực hiện nghiệp vụ từ chối bài đăng kèm lý do cụ thể
            var success = await _jobPostService.RejectJobAsync(id, moderatorId, reason);
            if (!success) return BadRequest("Cannot reject this job.");

            // Lấy thông tin job và gửi thông báo cho recruiter
            var job = await _jobPostService.GetJobPostByIdAsync(id);
            if (job != null)
            {
                var recruiters = await _db.Recruiters
                    .Include(r => r.RecruiterNavigation) // Include UserAccount to get Email
                    .Where(r => r.CompanyId == job.CompanyId)
                    .ToListAsync();

                foreach (var recruiter in recruiters)
                {
                    try
                    {
                        // 1. Gửi thông báo In-App
                        await _notificationService.SendNotificationAsync(
                            userId: recruiter.RecruiterId,
                            userType: "RECRUITER",
                            title: "Bài đăng bị từ chối",
                            message: $"Tin tuyển dụng \"{job.Title}\" của bạn đã bị từ chối. Lý do: {reason}",
                            type: "JOB_POST",
                            severity: "danger",
                            referenceId: id,
                            referenceType: "JobPost"
                        );

                        // 2. Gửi Email (nếu recruiter cho phép nhận email)
                        if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email))
                        {
                            if (recruiter.RecruiterNavigation.EmailNotificationsEnabled)
                            {
                                var emailSubject = $"[DevHub] Bài đăng bị từ chối: {job.Title}";
                                var emailContent = DevHub.Helpers.EmailHelper.GetBaseTemplate(
                                    "Bài đăng bị từ chối",
                                    $"<p>Xin chào <strong>{recruiter.FullName}</strong>,</p>" +
                                    $"<p>Rất tiếc, tin tuyển dụng <strong>\"{job.Title}\"</strong> của bạn đã bị từ chối duyệt.</p>" +
                                    $"<p><strong>Lý do:</strong> {reason}</p>" +
                                    $"<p>Vui lòng đăng nhập vào hệ thống để chỉnh sửa và gửi lại yêu cầu duyệt.</p>" +
                                    $"<p>Cảm ơn bạn đã đồng hành cùng DevHub!</p>"
                                );
                                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, emailSubject, emailContent);
                            }
                        }
                    }
                    catch { /* best-effort */ }
                }
            }

            var auditLog = new DevHub.Models.AuditLog
            {
                UserId = moderatorId,
                UserType = "Moderator",
                Action = "Từ chối bài đăng",
                EntityType = "JobPost",
                EntityId = id,
                OldValue = "Chờ duyệt",
                NewValue = "Từ chối (Lý do: " + reason + ")",
                CreatedAt = DateTime.UtcNow
            };
            _db.AuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // Action xử lý yêu cầu GET đến địa chỉ "moderator/job-approvals/detail/{id}" để xem chi tiết bài đăng dạng AJAX/JSON
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            // 1. Gọi Service lấy thông tin chi tiết của bài đăng dựa trên ID
            var job = await _jobPostService.GetJobPostByIdAsync(id);

            // 2. Nếu không tìm thấy bài đăng -> Trả về lỗi 404 Not Found
            if (job == null) return NotFound();

            // 3. Nếu tìm thấy, trích xuất các thông tin cần thiết và trả về dưới dạng định dạng dữ liệu JSON
            return Json(new
            {
                title = job.Title,
                description = job.Description,
                requirement = job.Requirement,
                benefit = job.Benefit,
                skill = job.Skill,
                salaryMin = job.SalaryMin,
                salaryMax = job.SalaryMax,
                location = job.Location,
                workingModel = job.WorkingModel,
                experienceLevel = job.ExperienceLevel,
                hiringQuota = job.HiringQuota,
                deadline = job.Deadline?.ToString("dd/MM/yyyy") // Định dạng lại ngày hạn chót thành dd/MM/yyyy nếu có giá trị
            });
        }
    }
}

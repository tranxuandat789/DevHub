
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

namespace DevHub.Controllers.Moderator
{
    // Cấu hình định tuyến (Route) chung cho các Action trong Controller này: "moderator/job-approvals"
    [Route("moderator/job-approvals")]
    // Giới hạn quyền truy cập: Chỉ những người dùng có Role là "Moderator" (Kiểm duyệt viên) mới được phép vào
    [Authorize(Roles = "Moderator")]
    public class JobApprovalController : Controller
    {
        // Khai báo dịch vụ xử lý nghiệp vụ bài đăng tuyển dụng (Service) dưới dạng chỉ đọc
        private readonly IJobPostService _jobPostService;

        // Hàm khởi tạo (Constructor) để tiêm dependency (Dependency Injection) của Service vào Controller
        public JobApprovalController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
        }

        // Action xử lý yêu cầu GET đến địa chỉ "moderator/job-approvals" để hiển thị danh sách bài đăng chờ duyệt
        [HttpGet("")]
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string? sortOrder)
        {
            // 1. Gọi Service lấy danh sách bài đăng đang chờ duyệt dựa theo bộ lọc (ngày tháng, sắp xếp)
            var pendingJobs = await _jobPostService.GetPendingJobsAsync(fromDate, toDate, sortOrder);

            // 2. Khởi tạo ViewModel để truyền dữ liệu và các tiêu chí lọc ra giao diện (View)
            var viewModel = new JobApprovalViewModel
            {
                JobPosts = pendingJobs,
                FromDate = fromDate,
                ToDate = toDate,
                SortOrder = sortOrder
            };

            // 3. Trả về View giao diện tương ứng kèm theo dữ liệu (ViewModel)
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

            // Nếu phê duyệt thất bại (Ví dụ: bài đăng không tồn tại hoặc không ở trạng thái pending) -> Trả về lỗi 400
            if (!success) return BadRequest("Cannot approve this job.");

            // 3. Phê duyệt thành công, trả về phản hồi HTTP 200 kèm mã JSON xác nhận
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

            // Nếu từ chối thất bại -> Trả về lỗi 400
            if (!success) return BadRequest("Cannot reject this job.");

            // 3. Từ chối thành công, trả về phản hồi HTTP 200 kèm mã JSON xác nhận
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

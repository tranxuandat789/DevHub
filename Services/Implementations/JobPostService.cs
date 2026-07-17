
//5/2/2026 
//file view duyet bai dang
//author: Hoang Minh Kien


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.Implementations;

// Lớp JobPostService triển khai từ giao diện IJobPostService để xử lý các nghiệp vụ (business logic) liên quan đến bài đăng tuyển dụng
public class JobPostService : IJobPostService
{
    private readonly IJobPostRepository _jobPostRepository;
    private readonly DevHub.Data.ItrecruitmentDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly DevHub.Helpers.EmailHelper _emailHelper;

    public JobPostService(
        IJobPostRepository jobPostRepository,
        DevHub.Data.ItrecruitmentDbContext context,
        INotificationService notificationService,
        DevHub.Helpers.EmailHelper emailHelper)
    {
        _jobPostRepository = jobPostRepository;
        _context = context;
        _notificationService = notificationService;
        _emailHelper = emailHelper;
    }

    // Lấy danh sách bài đăng (bao gồm pending, approved, rejected) có bộ lọc + sắp xếp + phân trang.
    public Task<(List<JobPost> Items, int TotalCount)> GetModeratorJobsAsync(int moderatorId, DateTime? fromDate, DateTime? toDate, string? sortOrder, int page, int pageSize)
        => _jobPostRepository.GetModeratorJobPostsAsync(moderatorId, fromDate, toDate, sortOrder, page, pageSize);

    // Tìm kiếm một bài đăng tuyển dụng dựa vào ID bài đăng
    public async Task<JobPost?> GetJobPostByIdAsync(int jobId)
    {
        // Trả về thông tin bài đăng hoặc null nếu không tìm thấy bài đăng nào có ID tương ứng
        return await _jobPostRepository.GetJobPostByIdAsync(jobId);
    }

    // Xử lý phê duyệt (Approve) một bài đăng tuyển dụng
    public async Task<bool> ApproveJobAsync(int jobId, int moderatorId)
    {
        // 1. Tìm thông tin bài đăng trong cơ sở dữ liệu dựa trên ID
        var job = await _jobPostRepository.GetJobPostByIdAsync(jobId);

        // Kiểm tra điều kiện: Nếu bài đăng không tồn tại HOẶC trạng thái hiện tại không phải là "pending" (chờ duyệt)
        // thì không thể phê duyệt -> Trả về false
        if (job == null || !string.Equals(job.Status, "PENDING", StringComparison.OrdinalIgnoreCase)) return false;

        // 2. Cập nhật lại các thông tin duyệt bài đăng
        job.Status = "APPROVED";          // Đổi trạng thái thành "approved" (đã phê duyệt)
        job.ApprovedAt = DateTime.Now;     // Ghi nhận thời gian phê duyệt là thời điểm hiện tại
        job.ModeratorId = moderatorId;     // Ghi nhận ID của người kiểm duyệt (moderator) bài viết này

        // 3. Gọi repository để lưu các thay đổi này vào cơ sở dữ liệu
        await _jobPostRepository.UpdateJobPostAsync(job);

        // 4. Nếu tin được duyệt lại sau khi recruiter chỉnh sửa, các ứng viên có đơn đang active
        //    (PENDING/APPROVED) đã bị "freeze" -> báo cho họ biết tin đã hoạt động trở lại.
        //    (Tin mới chưa có ứng viên nào nên sẽ không gửi cho ai.)
        try
        {
            await _jobPostRepository.NotifyApplicantsOnJobReApprovedAsync(jobId, job.Title);
        }
        catch
        {
            // Notification failure must not roll back the approval.
        }

        // 5. Gửi thông báo cho Recruiter của công ty
        try
        {
            var recruiters = await _context.Recruiters
                .Include(r => r.RecruiterNavigation)
                .Where(r => r.CompanyId == job.CompanyId)
                .ToListAsync();

            foreach (var r in recruiters)
            {
                var user = r.RecruiterNavigation;
                if (user != null)
                {
                    // In-app notification
                    await _notificationService.SendNotificationAsync(
                        user.UserId,
                        "RECRUITER",
                        "JOB_APPROVED",
                        $"Tin tuyển dụng '{job.Title}' đã được duyệt.",
                        $"/recruiter/job-posts/{job.JobId}"
                    );

                    // Email notification
                    if (user.EmailNotificationsEnabled)
                    {
                        string emailSubject = "DevHub - Tin tuyển dụng đã được duyệt";
                        string emailBody = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2 style='color: #4CAF50;'>Tin tuyển dụng đã được duyệt!</h2>
                            <p>Chào {r.FullName},</p>
                            <p>Tin tuyển dụng <strong>{job.Title}</strong> của bạn đã được kiểm duyệt viên phê duyệt.</p>
                            <p>Tin của bạn hiện đã hiển thị trên DevHub.</p>
                            <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                        </div>";
                        await _emailHelper.SendEmailAsync(user.Email, emailSubject, emailBody);
                    }
                }
            }
        }
        catch
        {
            // Bỏ qua lỗi thông báo để không ảnh hưởng luồng chính
        }

        // Phê duyệt thành công -> Trả về true
        return true;
    }

    // Xử lý từ chối (Reject) một bài đăng tuyển dụng
    public async Task<bool> RejectJobAsync(int jobId, int moderatorId, string reason)
    {
        // 1. Tìm thông tin bài đăng trong cơ sở dữ liệu dựa trên ID
        var job = await _jobPostRepository.GetJobPostByIdAsync(jobId);

        // Kiểm tra điều kiện: Nếu bài đăng không tồn tại HOẶC trạng thái hiện tại không phải là "pending" (chờ duyệt)
        // thì không thể từ chối -> Trả về false
        // Case-insensitive so it works regardless of how the status casing was stored.
        if (job == null || !string.Equals(job.Status, "PENDING", StringComparison.OrdinalIgnoreCase)) return false;

        // 2. Cập nhật lại các thông tin từ chối bài đăng
        job.Status = "REJECTED";           // Đổi trạng thái thành "rejected" (đã từ chối)
        job.RejectedReason = reason;       // Lưu lại lý do từ chối bài đăng
        job.ModeratorId = moderatorId;     // Ghi nhận ID của người kiểm duyệt thực hiện từ chối bài viết

        // 3. Gọi repository để lưu các thay đổi này vào cơ sở dữ liệu
        await _jobPostRepository.UpdateJobPostAsync(job);

        // 4. Gửi thông báo cho Recruiter của công ty
        try
        {
            var recruiters = await _context.Recruiters
                .Include(r => r.RecruiterNavigation)
                .Where(r => r.CompanyId == job.CompanyId)
                .ToListAsync();

            foreach (var r in recruiters)
            {
                var user = r.RecruiterNavigation;
                if (user != null)
                {
                    // In-app notification
                    await _notificationService.SendNotificationAsync(
                        user.UserId,
                        "RECRUITER",
                        "JOB_REJECTED",
                        $"Tin tuyển dụng '{job.Title}' đã bị từ chối duyệt.",
                        $"/recruiter/job-posts/{job.JobId}"
                    );

                    // Email notification
                    if (user.EmailNotificationsEnabled)
                    {
                        string emailSubject = "DevHub - Tin tuyển dụng bị từ chối";
                        string emailBody = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2 style='color: #F44336;'>Tin tuyển dụng chưa được duyệt</h2>
                            <p>Chào {r.FullName},</p>
                            <p>Tin tuyển dụng <strong>{job.Title}</strong> của bạn không được phê duyệt với lý do sau:</p>
                            <blockquote style='border-left: 4px solid #F44336; padding-left: 10px; color: #555;'>
                                {reason}
                            </blockquote>
                            <p>Vui lòng đăng nhập vào DevHub để chỉnh sửa và gửi lại yêu cầu.</p>
                            <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                        </div>";
                        await _emailHelper.SendEmailAsync(user.Email, emailSubject, emailBody);
                    }
                }
            }
        }
        catch
        {
            // Bỏ qua lỗi thông báo để không ảnh hưởng luồng chính
        }

        // Từ chối thành công -> Trả về true
        return true;
    }
}
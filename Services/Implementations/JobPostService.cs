
//5/2/2026 
//file view duyet bai dang
//author: Hoang Minh Kien


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

// Lớp JobPostService triển khai từ giao diện IJobPostService để xử lý các nghiệp vụ (business logic) liên quan đến bài đăng tuyển dụng
public class JobPostService : IJobPostService
{
    // Khai báo một trường chỉ đọc để lưu trữ repository, dùng để tương tác với cơ sở dữ liệu
    private readonly IJobPostRepository _jobPostRepository;

    // Hàm khởi tạo (Constructor) để tiêm dependency (Dependency Injection) của Repository vào Service
    public JobPostService(IJobPostRepository jobPostRepository)
    {
        _jobPostRepository = jobPostRepository;
    }

    // Lấy danh sách bài đăng đang chờ duyệt (Pending) có bộ lọc + sắp xếp + phân trang (ở SQL).
    public Task<(List<JobPost> Items, int TotalCount)> GetPendingJobsAsync(DateTime? fromDate, DateTime? toDate, string? sortOrder, int page, int pageSize)
        => _jobPostRepository.GetPendingJobPostsAsync(fromDate, toDate, sortOrder, page, pageSize);

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

        // Từ chối thành công -> Trả về true
        return true;
    }
}
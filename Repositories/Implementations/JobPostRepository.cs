
//5/2/2026 
//file view duyet bai dang
//author: Hoang Minh Kien



using System;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
    
namespace DevHub.Repositories.Implementations;

// Lớp JobPostRepository triển khai từ giao diện IJobPostRepository để thực hiện các thao tác CRUD trực tiếp với Cơ sở dữ liệu
public class JobPostRepository : IJobPostRepository
{
    // Khai báo đối tượng DbContext (Entity Framework Core) dùng để quản lý việc kết nối và truy vấn DB
    private readonly ItrecruitmentDbContext _context;

    // Hàm khởi tạo (Constructor) để tiêm dependency (Dependency Injection) của DbContext vào Repository
    public JobPostRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách bài đăng "pending" (chờ duyệt) kèm bộ lọc/sắp xếp + phân trang ngay ở SQL (Skip/Take).
    // Trả về (danh sách trang hiện tại, tổng số bài khớp bộ lọc).
    public async Task<(List<JobPost> Items, int TotalCount)> GetModeratorJobPostsAsync(int moderatorId, DateTime? fromDate, DateTime? toDate, string? sortOrder, int page, int pageSize)
    {
        // 1. Truy vấn cơ bản: lọc các bài đăng được assign cho moderator này, hoặc chưa assign nhưng đang PENDING.
        var query = _context.JobPosts
            .Include(j => j.Company)
            .Include(j => j.Position)
            .Include(j => j.Provinces)
            .Where(j => j.Status != null && 
                        (j.ModeratorId == moderatorId || (j.ModeratorId == null && j.Status.ToUpper() == "PENDING")));

        // 2. Bộ lọc theo ngày bắt đầu.
        if (fromDate.HasValue)
            query = query.Where(j => j.CreatedAt >= fromDate.Value.Date);

        // 3. Bộ lọc theo ngày kết thúc (đến hết 23:59:59 của ngày đó).
        if (toDate.HasValue)
        {
            var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(j => j.CreatedAt <= endOfDay);
        }

        // 4. Đếm tổng số bài khớp bộ lọc (trước khi phân trang).
        int totalCount = await query.CountAsync();

        // 5. Clamp trang về [1, totalPages] để Skip không vượt quá dữ liệu.
        if (pageSize < 1) pageSize = 10;
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        // 6. Sắp xếp (Sort)
        query = sortOrder switch
        {
            "oldest" => query.OrderBy(j => j.CreatedAt),
            _ => query.OrderByDescending(j => j.PriorityScore ?? 0).ThenByDescending(j => j.CreatedAt)
        };  // 7. Phân trang ở SQL và trả về.
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    // Tìm kiếm một bài đăng tuyển dụng cụ thể dựa trên ID (Khóa chính)
    public async Task<JobPost?> GetJobPostByIdAsync(int id)
    {
        // Trả về bài đăng đầu tiên tìm thấy thỏa mãn điều kiện JobId == id, hoặc trả về null nếu không tìm thấy
        return await _context.JobPosts
            .Include(j => j.Provinces)
            .FirstOrDefaultAsync(j => j.JobId == id);
    }

    // Cập nhật thông tin của một bài đăng tuyển dụng hiện có
    public async Task UpdateJobPostAsync(JobPost job)
    {
        // Đánh dấu thực thể (entity) này là đã bị chỉnh sửa (Modified) trong DbContext
        _context.JobPosts.Update(job);

        // Thực thi việc lưu và cập nhật các thay đổi này xuống Cơ sở dữ liệu một cách bất đồng bộ
        await _context.SaveChangesAsync();
    }

    // Notify candidates with an active (PENDING/APPROVED) application that the re-reviewed job is live again.
    public async Task NotifyApplicantsOnJobReApprovedAsync(int jobId, string jobTitle)
    {
        var targets = await _context.Applications
            .AsNoTracking()
            .Where(a => a.JobId == jobId
                     && a.Status != null
                     && (a.Status.ToUpper() == "PENDING" || a.Status.ToUpper() == "APPROVED"))
            .Select(a => new { a.CandidateId, a.ApplicationId })
            .ToListAsync();

        if (targets.Count == 0) return;

        var notifications = targets.Select(t => new Notification
        {
            UserId = t.CandidateId,
            UserType = "CANDIDATE",
            Type = "APPLICATION",
            Title = "Tin tuyển dụng đã được cập nhật",
            Message = $"Tin \"{jobTitle}\" đã được cập nhật và tiếp tục hoạt động. " +
                      "Vui lòng xem lại thông tin yêu cầu trước khi tiếp tục quá trình ứng tuyển.",
            ReferenceType = "Application",
            ReferenceId = t.ApplicationId,
            SeverityLevel = "info",
            IsRead = false,
            CreatedAt = DateTime.Now
        }).ToList();

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();
    }
}

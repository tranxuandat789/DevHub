
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

    // Lấy danh sách các bài đăng tuyển dụng có trạng thái "pending" (chờ duyệt) kèm theo bộ lọc và sắp xếp
    public async Task<List<JobPost>> GetPendingJobPostsAsync(DateTime? fromDate, DateTime? toDate, string? sortOrder)
    {
        // 1. Khởi tạo truy vấn cơ bản: Lấy dữ liệu từ bảng JobPosts, kết hợp (Eager Loading) bảng Recruiter và Position,
        // sau đó lọc ra các bài đăng có trạng thái Status là "pending"
        var query = _context.JobPosts
            .Include(j => j.Recruiter)
            .Include(j => j.Position)
            .Where(j => j.Status != null && j.Status.ToUpper() == "PENDING");

        // 2. Bộ lọc theo ngày bắt đầu (fromDate): Nếu có giá trị, chỉ lấy các bài viết tạo từ ngày này trở đi (so sánh phần Ngày .Date)
        if (fromDate.HasValue)
            query = query.Where(j => j.CreatedAt >= fromDate.Value.Date);

        // 3. Bộ lọc theo ngày kết thúc (toDate): Nếu có giá trị, tính toán thời điểm cuối cùng của ngày đó (23:59:59.999...)
        // để đảm bảo không bỏ sót các bài đăng được tạo trong ngày kết thúc
        if (toDate.HasValue)
        {
            var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(j => j.CreatedAt <= endOfDay);
        }

        // 4. Sắp xếp kết quả: 
        // Nếu tham số sortOrder là "oldest", sắp xếp tăng dần theo thời gian tạo (bài cũ lên trước)
        // Ngược lại (mặc định hoặc truyền giá trị khác), sắp xếp giảm dần (bài mới lên trước)
        if (sortOrder == "oldest")
            query = query.OrderBy(j => j.CreatedAt);
        else
            query = query.OrderByDescending(j => j.CreatedAt);

        // 5. Thực thi truy vấn xuống Cơ sở dữ liệu và trả về danh sách kết quả một cách bất đồng bộ
        return await query.ToListAsync();
    }

    // Tìm kiếm một bài đăng tuyển dụng cụ thể dựa trên ID (Khóa chính)
    public async Task<JobPost?> GetJobPostByIdAsync(int id)
    {
        // Trả về bài đăng đầu tiên tìm thấy thỏa mãn điều kiện JobId == id, hoặc trả về null nếu không tìm thấy
        return await _context.JobPosts.FirstOrDefaultAsync(j => j.JobId == id);
    }

    // Cập nhật thông tin của một bài đăng tuyển dụng hiện có
    public async Task UpdateJobPostAsync(JobPost job)
    {
        // Đánh dấu thực thể (entity) này là đã bị chỉnh sửa (Modified) trong DbContext
        _context.JobPosts.Update(job);

        // Thực thi việc lưu và cập nhật các thay đổi này xuống Cơ sở dữ liệu một cách bất đồng bộ
        await _context.SaveChangesAsync();
    }
}
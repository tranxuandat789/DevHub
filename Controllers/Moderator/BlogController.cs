using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Controllers.Moderator
{
    [Route("ModeratorBlog")]
    [Authorize(Roles = "Moderator")]
    public class BlogController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly IWebHostEnvironment _env;
        private readonly ItrecruitmentDbContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public BlogController(IBlogPostService blogPostService, IWebHostEnvironment env, ItrecruitmentDbContext context, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _blogPostService = blogPostService;
            _env = env;
            _context = context;
            _config = config;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string keyword, string dateFrom, string status, string sortBy, int page = 1)
        {
            int pageSize = 10;
            var result = await _blogPostService.GetFilteredBlogsAsync(keyword, dateFrom, status, sortBy, page, pageSize, null, true);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Keyword = keyword;
            ViewBag.DateFrom = dateFrom;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;

            return View("~/Views/Moderator/Blog/Index.cshtml", result.Blogs);
        }

        // Preview: moderator xem bài bất kể status (không filter Status == 1)
        [HttpGet("Preview/{id}")]
        public async Task<IActionResult> Preview(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();
            return View("~/Views/Blog/Details.cshtml", blog);
        }

        [HttpPost("Approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var moderatorIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int moderatorId = 0;
            int.TryParse(moderatorIdClaim, out moderatorId);

            // Lấy blog TRƯỚC khi approve (để có Title)
            var blog = await _blogPostService.GetPostByIdAsync(id);
            var success = await _blogPostService.ApprovePostAsync(id, moderatorId);
            if (!success) return NotFound();

            // Gửi thông báo cho recruiter
            if (blog?.AuthorId != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = blog.AuthorId.Value,
                    UserType = "RECRUITER",
                    Title = "Bài viết đã được duyệt",
                    Message = $"Bài viết \"{blog.Title}\" của bạn đã được duyệt và xuất bản thành công.",
                    Type = "BLOG_APPROVED",
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    ReferenceId = blog.BlogId,
                    ReferenceType = "BLOG"
                });
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost("Reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromForm] string reason)
        {
            var moderatorIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int moderatorId = 0;
            int.TryParse(moderatorIdClaim, out moderatorId);

            var success = await _blogPostService.RejectPostAsync(id, moderatorId, reason);
            if (!success) return NotFound();

            return Ok();
        }

        [HttpPost("ToggleVisibility/{id}")]
        public async Task<IActionResult> ToggleVisibility(int id, [FromForm] string reason)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            if (blog.Status == 1)
            {
                // Mod ẩn → Status 5 (khác Status 2 là recruiter tự ẩn)
                var dbBlog = await _context.BlogPosts.FindAsync(id);
                if (dbBlog == null) return NotFound();
                dbBlog.Status = 5;
                await _context.SaveChangesAsync();

                // Gửi mail thông báo
                var recruiterEmail = blog.AuthorRecruiter?.RecruiterNavigation?.Email;
                var recruiterName = blog.AuthorRecruiter?.FullName ?? blog.Author ?? "Nhà tuyển dụng";
                var blogTitle = blog.Title;
                if (!string.IsNullOrEmpty(recruiterEmail))
                {
                    var reasonText = string.IsNullOrWhiteSpace(reason) ? "Không có lý do cụ thể." : reason;
                    var body = $@"
<div style='font-family:Inter,sans-serif;max-width:600px;margin:auto;padding:32px;background:#f9fafb;border-radius:12px;'>
  <h2 style='color:#1a1a2e;'>Bài viết đã bị ẩn</h2>
  <p style='color:#555;'>Xin chào <strong>{recruiterName}</strong>,</p>
  <p style='color:#555;'>Bài viết <strong>&quot;{blogTitle}&quot;</strong> của bạn đã bị ẩn khỏi trang cẩm nang nghề nghiệp.</p>
  <div style='background:#fff3cd;border-left:4px solid #ffc107;padding:16px;border-radius:8px;margin:16px 0;'>
    <p style='margin:0;color:#856404;'><strong>Lý do:</strong> {reasonText}</p>
  </div>
  <p style='color:#555;'>Nếu bạn có thắc mắc, vui lòng liên hệ với chúng tôi qua email hỗ trợ.</p>
  <p style='color:#555;'>Trân trọng,<br/><strong>Đội ngũ DevHub</strong></p>
</div>";
                    await new DevHub.Helpers.EmailHelper(_config).SendEmailAsync(recruiterEmail, "Thông báo: Bài viết bị ẩn", body);
                }
            }
            else
            {
                // Status 5 hoặc 2 → hiện lại (Status 1)
                var dbBlog = await _context.BlogPosts.FindAsync(id);
                if (dbBlog == null) return NotFound();
                dbBlog.Status = 1;
                dbBlog.PublishedAt ??= DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id, [FromForm] string reason)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            // Lấy email recruiter trước khi xóa
            var recruiterEmail = blog.AuthorRecruiter?.RecruiterNavigation?.Email;
            var recruiterName = blog.AuthorRecruiter?.FullName ?? blog.Author ?? "Nhà tuyển dụng";
            var blogTitle = blog.Title;

            var success = await _blogPostService.DeletePostAsync(id);
            if (!success) return NotFound();

            // Gửi mail thông báo nếu có email
            if (!string.IsNullOrEmpty(recruiterEmail))
            {
                var reasonText = string.IsNullOrWhiteSpace(reason) ? "Không có lý do cụ thể." : reason;
                var body = $@"
<div style='font-family:Inter,sans-serif;max-width:600px;margin:auto;padding:32px;background:#f9fafb;border-radius:12px;'>
  <h2 style='color:#1a1a2e;'>Bài viết đã bị xóa</h2>
  <p style='color:#555;'>Xin chào <strong>{recruiterName}</strong>,</p>
  <p style='color:#555;'>Bài viết <strong>&quot;{blogTitle}&quot;</strong> của bạn đã bị xóa bởi quản trị viên.</p>
  <div style='background:#fff3cd;border-left:4px solid #ffc107;padding:16px;border-radius:8px;margin:16px 0;'>
    <p style='margin:0;color:#856404;'><strong>Lý do:</strong> {reasonText}</p>
  </div>
  <p style='color:#555;'>Nếu bạn có thắc mắc, vui lòng liên hệ với chúng tôi qua email hỗ trợ.</p>
  <p style='color:#555;'>Trân trọng,<br/><strong>Đội ngũ DevHub</strong></p>
</div>";
                await new DevHub.Helpers.EmailHelper(_config).SendEmailAsync(recruiterEmail, "Thông báo: Bài viết bị xóa", body);
            }

            return Ok();
        }
    }
}

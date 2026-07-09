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
            var result = await _blogPostService.GetFilteredBlogsAsync(keyword, dateFrom, status, sortBy, page, pageSize, true);

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
            ViewData["IsPreview"] = true;
            return View("~/Views/Moderator/Blog/Details.cshtml", blog);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Moderator/Blog/Create.cshtml", new DevHub.ViewModels.Moderator.CreateBlogViewModel());
        }


        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DevHub.ViewModels.Moderator.CreateBlogViewModel model, string actionType)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Moderator/Blog/Create.cshtml", model);
            }

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim)) return Unauthorized();
            int publisherId = int.Parse(adminIdClaim);

            string? thumbnailUrl = model.ThumbnailUrl;

            if (model.ThumbnailImage != null && model.ThumbnailImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = System.IO.Path.GetExtension(model.ThumbnailImage.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ThumbnailImage", "Chỉ hỗ trợ file ảnh (.jpg, .png, .gif, .webp)");
                    return View("~/Views/Moderator/Blog/Create.cshtml", model);
                }

                if (model.ThumbnailImage.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ThumbnailImage", "Kích thước ảnh không được vượt quá 5MB");
                    return View("~/Views/Moderator/Blog/Create.cshtml", model);
                }

                var uploadsFolder = System.IO.Path.Combine(_env.WebRootPath, "uploads", "blog", "thumbnails");
                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    await model.ThumbnailImage.CopyToAsync(fileStream);
                }

                thumbnailUrl = $"/uploads/blog/thumbnails/{uniqueFileName}";
            }

            bool isPublished = actionType == "publish";
            await _blogPostService.CreateBlogPostAsync(publisherId, model.Title, model.Content, thumbnailUrl, model.Tag, isPublished, model.AuthorName);

            TempData["SuccessMessage"] = isPublished ? "Đăng bài viết thành công!" : "Lưu bản nháp thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                try
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = System.IO.Path.GetExtension(upload.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                        return Json(new { success = false, message = "Định dạng file không hợp lệ." });

                    var uploadsFolder = System.IO.Path.Combine(_env.WebRootPath, "uploads", "blog", "content");
                    if (!System.IO.Directory.Exists(uploadsFolder))
                        System.IO.Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/blog/content/{uniqueFileName}";
                    return Json(new { success = true, url = relativePath });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Không có tệp nào được chọn" });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (blog.PublisherId.ToString() != adminIdClaim)
                return Unauthorized();

            var viewModel = new DevHub.ViewModels.Moderator.EditBlogViewModel
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Content = blog.Content,
                Tag = blog.Tag,
                AuthorName = blog.AuthorName,
                ThumbnailUrl = blog.ThumbnailUrl,
                CreatedAt = blog.CreatedAt
            };

            return View("~/Views/Moderator/Blog/Edit.cshtml", viewModel);
        }

        [HttpPost("EditPost/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, DevHub.ViewModels.Moderator.EditBlogViewModel model, string actionType)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Moderator/Blog/Edit.cshtml", model);
            }

            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (blog.PublisherId.ToString() != adminIdClaim)
                return Unauthorized();

            string? thumbnailUrl = model.ThumbnailUrl ?? blog.ThumbnailUrl;

            if (model.ThumbnailImage != null && model.ThumbnailImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = System.IO.Path.GetExtension(model.ThumbnailImage.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ThumbnailImage", "Chỉ hỗ trợ file ảnh (.jpg, .png, .gif, .webp)");
                    return View("~/Views/Moderator/Blog/Edit.cshtml", model);
                }

                if (model.ThumbnailImage.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ThumbnailImage", "Kích thước ảnh không được vượt quá 5MB");
                    return View("~/Views/Moderator/Blog/Edit.cshtml", model);
                }

                var uploadsFolder = System.IO.Path.Combine(_env.WebRootPath, "uploads", "blog", "thumbnails");
                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    await model.ThumbnailImage.CopyToAsync(fileStream);
                }

                thumbnailUrl = $"/uploads/blog/thumbnails/{uniqueFileName}";
            }

            bool isPublished = actionType == "publish";
            await _blogPostService.UpdateBlogPostAsync(id, model.Title, model.Content, thumbnailUrl, model.Tag, isPublished, model.AuthorName);

            TempData["SuccessMessage"] = isPublished ? "Cập nhật và xuất bản bài viết thành công!" : "Đã cập nhật bản nháp!";
            return RedirectToAction("Index");
        }

        [HttpPost("ToggleVisibility/{id}")]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            if (blog.Status == 1)
            {
                // Ẩn bài
                var dbBlog = await _context.BlogPosts.FindAsync(id);
                if (dbBlog == null) return NotFound();
                dbBlog.Status = 2; // Hidden
                await _context.SaveChangesAsync();
            }
            else
            {
                // Hiện lại
                var dbBlog = await _context.BlogPosts.FindAsync(id);
                if (dbBlog == null) return NotFound();
                dbBlog.Status = 1; // Published
                dbBlog.PublishedAt ??= DateTime.Now;
                await _context.SaveChangesAsync();
            }

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            var auditLog = new AuditLog
            {
                UserId = modId,
                UserType = "Moderator",
                Action = "Đổi trạng thái Blog",
                EntityType = "BlogPost",
                EntityId = id,
                OldValue = blog.Status.ToString(),
                NewValue = (blog.Status == 1 ? "2" : "1"),
                CreatedAt = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            var success = await _blogPostService.DeletePostAsync(id);
            if (!success) return NotFound();

            var modIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(modIdClaim, out int modId);
            var auditLog = new AuditLog
            {
                UserId = modId,
                UserType = "Moderator",
                Action = "Xóa Blog",
                EntityType = "BlogPost",
                EntityId = id,
                OldValue = "Tồn tại",
                NewValue = "Đã xóa",
                CreatedAt = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}

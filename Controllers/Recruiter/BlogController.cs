using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DevHub.Controllers.Recruiter
{
    [Route("RecruiterBlog")]
    [Authorize(Roles = "RECRUITER")]
    public class BlogController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly IWebHostEnvironment _env;

        public BlogController(IBlogPostService blogPostService, IWebHostEnvironment env)
        {
            _blogPostService = blogPostService;
            _env = env;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string keyword, string dateFrom, string status, string sortBy, int page = 1)
        {
            int pageSize = 10;
            
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int recruiterId = 0;
            int.TryParse(recruiterIdClaim, out recruiterId);

            var result = await _blogPostService.GetFilteredBlogsAsync(keyword, dateFrom, status, sortBy, page, pageSize, recruiterId);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Keyword = keyword;
            ViewBag.DateFrom = dateFrom;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;

            return View("~/Views/Recruiter/Blog/Index.cshtml", result.Blogs);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.AuthorName = User.FindFirst("FullName")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            return View("~/Views/Recruiter/Blog/Create.cshtml");
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] BlogPostCreateViewModel model)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var fullNameClaim = User.FindFirst("FullName")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            
            int recruiterId = 0;
            int.TryParse(recruiterIdClaim, out recruiterId);

            model.Author = fullNameClaim;
            ModelState.Remove("Author");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(string.Join(" ", errors));
            }

            try
            {
                await _blogPostService.CreatePostAsync(model, null, recruiterId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { success = false, message = errorMsg });
            }
        }

        [HttpPost("UploadThumbnail")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                        return Json(new { success = false, message = "Định dạng ảnh không hợp lệ." });

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "blog", "thumbnails");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/blog/thumbnails/{uniqueFileName}";
                    return Json(new { success = true, url = relativePath });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Không có tệp nào được chọn" });
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                try
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(upload.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                        return Json(new { uploaded = 0, error = new { message = "Định dạng ảnh không hợp lệ." } });

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "blog", "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/blog/images/{uniqueFileName}";
                    return Json(new { url = relativePath });
                }
                catch (Exception ex)
                {
                    return Json(new { uploaded = 0, error = new { message = ex.Message } });
                }
            }
            return Json(new { uploaded = 0, error = new { message = "Không có tệp nào được chọn" } });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null)
                return NotFound();
            
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int recruiterId = 0;
            int.TryParse(recruiterIdClaim, out recruiterId);
            
            if (blog.AuthorId != recruiterId)
            {
                return Unauthorized();
            }
            
            return View("~/Views/Recruiter/Blog/Edit.cshtml", blog);
        }

        [HttpPost("EditPost/{id}")]
        public async Task<IActionResult> EditPost(int id, [FromForm] BlogPostEditViewModel model)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var fullNameClaim = User.FindFirst("FullName")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            int recruiterId = 0;
            int.TryParse(recruiterIdClaim, out recruiterId);
            
            model.Author = fullNameClaim;
            ModelState.Remove("Author");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(string.Join(" ", errors));
            }
            
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            if (blog.AuthorId != recruiterId)
            {
                return Unauthorized();
            }

            var success = await _blogPostService.EditPostAsync(id, model);
            if (!success)
                return NotFound();

            return Json(new { success = true });
        }

        [HttpPost("ToggleVisibility/{id}")]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int recruiterId = 0;
            int.TryParse(recruiterIdClaim, out recruiterId);
            
            if (blog.AuthorId != recruiterId)
            {
                return Unauthorized();
            }

            // Không cho phép mở lại bài đã bị ẩn bởi mod
            if (blog.Status == 5)
            {
                return StatusCode(403, "Bài viết này đã bị ẩn bởi quản trị viên. Bạn không thể tự mở lại.");
            }
            
            var success = await _blogPostService.ToggleVisibilityAsync(id);
            if (!success) return NotFound();

            return Ok();
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int recruiterId = 0;
            int.TryParse(recruiterIdClaim, out recruiterId);
            
            if (blog.AuthorId != recruiterId)
            {
                return Unauthorized();
            }

            var success = await _blogPostService.DeletePostAsync(id);
            if (!success) return NotFound();

            return Ok();
        }
    }
}

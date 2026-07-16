using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using DevHub.Validators.ViewModels.Recruiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Controllers.Recruiter
{
    [Route("Recruiter/Article")]
    [Authorize(AuthenticationSchemes = "EmployerCookies")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IWebHostEnvironment _env;
        private readonly IAuthService _authService;

        public ArticleController(IArticleService articleService, IWebHostEnvironment env, IAuthService authService)
        {
            _articleService = articleService;
            _env = env;
            _authService = authService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string keyword, string dateFrom, string status, string sortBy, int page = 1)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser != null && dbUser.Recruiter != null)
            {
                // Removed company verification check
            }

            var allArticles = await _articleService.GetArticlesForRecruiterAsync(recruiterId);

            if (!string.IsNullOrEmpty(keyword))
                allArticles = allArticles.Where(a => a.Title != null && a.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(dateFrom) && DateTime.TryParse(dateFrom, out var dt))
                allArticles = allArticles.Where(a => a.CreatedAt.HasValue && a.CreatedAt.Value.Date >= dt.Date).ToList();

            if (!string.IsNullOrEmpty(status) && status != "All")
                allArticles = allArticles.Where(a => string.Equals(a.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();

            if (sortBy == "oldest")
                allArticles = allArticles.OrderBy(a => a.CreatedAt).ToList();
            else
                allArticles = allArticles.OrderByDescending(a => a.CreatedAt).ToList();

            int pageSize = 10;
            var totalItems = allArticles.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var pagedArticles = allArticles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Keyword = keyword;
            ViewBag.DateFrom = dateFrom;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;

            return View("~/Views/Recruiter/Article/Index.cshtml", pagedArticles);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            var dbUser = await _authService.FindUserByEmailAsync(email);
            if (dbUser != null && dbUser.Recruiter != null)
            {
                // Removed company verification and profile completion check to allow posting immediately
            }

            ViewBag.AuthorName = dbUser?.Recruiter?.Company?.CompanyName ?? User.FindFirst("FullName")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";
            return View("~/Views/Recruiter/Article/Create.cshtml");
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] ArticleCreateViewModel model)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(string.Join(" ", errors));
            }

            try
            {
                await _articleService.CreateArticleAsync(recruiterId, model.Title, model.Content, model.ThumbnailUrl ?? "", model.ActionType ?? "publish");
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

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "article", "thumbnails");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/article/thumbnails/{uniqueFileName}";
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

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "article", "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/article/images/{uniqueFileName}";
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
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();
            
            return View("~/Views/Recruiter/Article/Edit.cshtml", article);
        }

        [HttpPost("EditPost/{id}")]
        public async Task<IActionResult> EditPost(int id, [FromForm] ArticleEditViewModel model)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(string.Join(" ", errors));
            }
            
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null) return NotFound();

            try
            {
                await _articleService.UpdateArticleAsync(recruiterId, id, model.Title, model.Content, model.ThumbnailUrl ?? "", model.ActionType ?? "publish");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { success = false, message = errorMsg });
            }
        }

        [HttpPost("SubmitReview/{id}")]
        public async Task<IActionResult> SubmitReview(int id)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            try
            {
                await _articleService.SubmitArticleForReviewAsync(recruiterId, id);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("ToggleVisibility/{id}")]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            try
            {
                await _articleService.ToggleArticleVisibilityAsync(recruiterId, id);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            try
            {
                await _articleService.DeleteArticleAsync(recruiterId, id);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var recruiterIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            int recruiterId = int.Parse(recruiterIdClaim);

            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            var recruiter = await _authService.FindUserByEmailAsync(email);
            if (article.CompanyId != recruiter?.Recruiter?.CompanyId)
                return Forbid();

            return View("~/Views/Recruiter/Article/Detail.cshtml", article);
        }
    }
}

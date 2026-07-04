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
            return View("~/Views/Blog/Details.cshtml", blog);
        }



        [HttpPost("ToggleVisibility/{id}")]
        public async Task<IActionResult> ToggleVisibility(int id, [FromForm] string reason)
        {
            var blog = await _blogPostService.GetPostByIdAsync(id);
            if (blog == null) return NotFound();

            if (blog.Status == 1)
            {
                // Mod ẩn → Status 5
                var dbBlog = await _context.BlogPosts.FindAsync(id);
                if (dbBlog == null) return NotFound();
                dbBlog.Status = 5;
                await _context.SaveChangesAsync();
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

            var success = await _blogPostService.DeletePostAsync(id);
            if (!success) return NotFound();


            return Ok();
        }
    }
}

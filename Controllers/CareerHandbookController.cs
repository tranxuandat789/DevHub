using DevHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DevHub.Controllers
{
    public class CareerHandbookController : Controller
    {
        private readonly ItrecruitmentDbContext _context;

        public CareerHandbookController(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        [HttpGet("CareerHandbook")]
        public async Task<IActionResult> Index(int page = 1, string search = null, string tag = null)
        {
            int pageSize = 9; // 9 items per page (3x3 grid)
            var query = _context.BlogPosts
                .Include(b => b.Publisher)
                .Where(b => b.Status == 1);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(b => b.Title != null && b.Title.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                tag = tag.Trim();
                query = query.Where(b => b.Tag.Contains(tag));
            }

            query = query.OrderByDescending(b => b.PublishedAt ?? b.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allTags = await _context.BlogPosts
                .Where(b => b.Status == 1 && !string.IsNullOrEmpty(b.Tag))
                .Select(b => b.Tag)
                .ToListAsync();

            var uniqueTags = allTags
                .SelectMany(t => t.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .Take(5) // Lấy top 5 tags để hiển thị
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchKeyword = search;
            ViewBag.CurrentTag = tag;
            ViewBag.Tags = uniqueTags;

            return View("~/Views/Blog/Index.cshtml", blogs);
        }

        [HttpGet("CareerHandbook/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var blog = await _context.BlogPosts
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Slug == slug && b.Status == 1);

            if (blog == null)
            {
                return NotFound();
            }

            var relatedBlogs = await _context.BlogPosts
                .Include(b => b.Publisher)
                .Where(b => b.BlogId != blog.BlogId && b.Status == 1)
                .OrderByDescending(b => b.PublishedAt ?? b.CreatedAt)
                .Take(2)
                .ToListAsync();

            ViewBag.RelatedBlogs = relatedBlogs;

            return View("~/Views/Blog/Details.cshtml", blog);
        }
    }
}

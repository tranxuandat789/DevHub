using DevHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DevHub.Controllers
{
    public class BlogController : Controller
    {
        private readonly ItrecruitmentDbContext _context;

        public BlogController(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        [HttpGet("Blog")]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 9; // 9 items per page (3x3 grid)
            var query = _context.BlogPosts
                .Include(b => b.Publisher)
                .Where(b => b.Status == 1)
                .OrderByDescending(b => b.PublishedAt ?? b.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View("~/Views/Blog/Index.cshtml", blogs);
        }

        [HttpGet("Blog/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            var blog = await _context.BlogPosts
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.Slug == slug && b.Status == 1);

            if (blog == null)
            {
                return NotFound();
            }

            return View("~/Views/Blog/Details.cshtml", blog);
        }
    }
}

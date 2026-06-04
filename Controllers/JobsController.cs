using DevHub.Data;
using DevHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Controllers
{
    public class JobsController : Controller
    {
        private readonly ItrecruitmentDbContext _context;

        public JobsController(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword, string? location, int page = 1)
        {
            int pageSize = 10;
            var query = _context.JobPosts
                .Include(j => j.Recruiter)
                .Include(j => j.Position)
                .Where(j => j.Status == "Approved"); // Assuming approved jobs only

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(j => j.Title.Contains(keyword) || j.Skill.Contains(keyword) || j.Recruiter.CompanyName.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(j => j.Location.Contains(location));
            }

            // Simple Pagination
            int totalJobs = await query.CountAsync();
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.Location = location;
            ViewBag.TotalJobs = totalJobs;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalJobs / pageSize);

            return View("~/Views/Candidate/Job/Index.cshtml", jobs);
        }

        public IActionResult Details(int id = 1)
        {
            return View("~/Views/Candidate/Job/Details.cshtml");
        }
    }
}

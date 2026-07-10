using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.ViewModels.Articles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace DevHub.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ItrecruitmentDbContext _db;

        public ArticlesController(ItrecruitmentDbContext db)
        {
            _db = db;
        }

        [HttpGet("articles")]
        public async Task<IActionResult> Index(
            string? search,
            int? companyId,
            string sort = "newest",
            int page = 1)
        {
            const int pageSize = 10;

            var query = _db.Articles
                .Where(a => a.Status == "PUBLISHED")
                .Include(a => a.Company)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a => a.Title!.Contains(search));

            if (companyId.HasValue)
                query = query.Where(a => a.CompanyId == companyId.Value);

            query = sort == "oldest"
                ? query.OrderBy(a => a.CreatedAt)
                : query.OrderByDescending(a => a.CreatedAt);

            var total = await query.CountAsync();

            var articles = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Top công ty theo số bài (cho sidebar)
            var companyFilters = await _db.Articles
                .Where(a => a.Status == "PUBLISHED")
                .GroupBy(a => new { a.CompanyId, a.Company!.CompanyName, a.Company.CompanyLogoUrl })
                .Select(g => new
                {
                    g.Key.CompanyId,
                    g.Key.CompanyName,
                    g.Key.CompanyLogoUrl,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(8)
                .ToListAsync();

            var vm = new ArticleListViewModel
            {
                Articles     = articles,
                CompanyFilters = companyFilters
                    .Select(c => (c.CompanyId ?? 0, c.CompanyName, c.CompanyLogoUrl, c.Count))
                    .ToList(),
                Search       = search,
                CompanyId    = companyId,
                Sort         = sort,
                Page         = page,
                PageSize     = pageSize,
                TotalCount   = total
            };

            return View("~/Views/Candidate/Article/Index.cshtml", vm);
        }

        [HttpGet("articles/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var article = await _db.Articles
                .Include(a => a.Company)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null) return NotFound();

            if (article.Status != "PUBLISHED")
            {
                bool canViewHidden = false;

                // Check Admin/Moderator
                var adminAuth = await HttpContext.AuthenticateAsync("AdminCookies");
                if (adminAuth.Succeeded)
                {
                    canViewHidden = true;
                }
                else
                {
                    // Check Employer
                    var empAuth = await HttpContext.AuthenticateAsync("EmployerCookies");
                    if (empAuth.Succeeded)
                    {
                        var recruiterIdClaim = empAuth.Principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(recruiterIdClaim, out int recruiterId))
                        {
                            var recruiter = await _db.Recruiters.FirstOrDefaultAsync(r => r.RecruiterId == recruiterId);
                            if (recruiter != null && recruiter.CompanyId == article.CompanyId)
                            {
                                canViewHidden = true;
                            }
                        }
                    }
                }

                if (!canViewHidden)
                {
                    return NotFound();
                }
            }

            return View("~/Views/Candidate/Article/Detail.cshtml", article);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ItrecruitmentDbContext _db;

        public CompaniesController(ICompanyService companyService, ItrecruitmentDbContext db)
        {
            _companyService = companyService;
            _db = db;
        }

        [HttpGet("Company/Search")]
        [HttpGet("companies")] // Keep legacy route mapping
        public async Task<IActionResult> Index([FromQuery] CompanySearchInputViewModel input)
        {
            var model = await _companyService.SearchCompaniesAsync(input);
            return View("~/Views/Candidate/Company/Index.cshtml", model);
        }

        [HttpGet("Company/Detail/{id}")]
        [HttpGet("Companies/Details/{id}")] // Keep legacy route mapping
        public async Task<IActionResult> Details(int id)
        {
            int? candidateId = null;
            if (User.Identity?.IsAuthenticated == true && (User.IsInRole("Candidate") || User.IsInRole("CANDIDATE")))
            {
                var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (idClaim != null && int.TryParse(idClaim.Value, out int cid))
                {
                    candidateId = cid;
                }
            }

            var model = await _companyService.GetCompanyDetailsAsync(id, candidateId);
            if (model == null)
            {
                return NotFound();
            }
            return View("~/Views/Candidate/Company/Details.cshtml", model);
        }

        [HttpGet("Company/NavData")]
        public async Task<IActionResult> NavData()
        {
            // Top 5 công ty theo số bài viết PUBLISHED
            var topCompanies = await _db.Articles
                .Where(a => a.Status == "PUBLISHED")
                .GroupBy(a => a.Company)
                .Select(g => new
                {
                    companyId   = g.Key!.CompanyId,
                    companyName = g.Key.CompanyName,
                    logoUrl     = g.Key.CompanyLogoUrl,
                    articleCount = g.Count()
                })
                .OrderByDescending(x => x.articleCount)
                .Take(5)
                .ToListAsync();

            // 3 bài viết mới nhất
            var recentArticles = await _db.Articles
                .Where(a => a.Status == "PUBLISHED")
                .Include(a => a.Company)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new
                {
                    articleId    = a.ArticleId,
                    title        = a.Title,
                    thumbnailUrl = a.ThumbnailUrl,
                    companyId    = a.CompanyId,
                    companyName  = a.Company != null ? a.Company.CompanyName : "",
                    approvedAt   = a.CreatedAt
                })
                .ToListAsync();

            return Json(new { topCompanies, recentArticles });
        }
    }
}

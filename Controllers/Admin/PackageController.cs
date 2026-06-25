using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevHub.Controllers.Admin
{
    [Route("admin/packages")]
    [Authorize(Roles = "Admin")]
    public class PackageController : Controller
    {
        private readonly IServicePackageService _packageService;

        public PackageController(IServicePackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = "", string statusFilter = "all", string sortOrder = "newest")
        {
            var (packages, totalCount) = await _packageService.GetAllPackagesAsync(searchTerm, statusFilter, sortOrder, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.SortOrder = sortOrder;

            return View(packages);
        }
    }
}

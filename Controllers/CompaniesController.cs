using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Company;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
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
            var model = await _companyService.GetCompanyDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View("~/Views/Candidate/Company/Details.cshtml", model);
        }
    }
}

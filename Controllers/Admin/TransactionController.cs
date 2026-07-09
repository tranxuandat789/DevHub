using System;
using System.Threading.Tasks;
using DevHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers.Admin;

[Route("AdminTransaction")]
[Authorize(Roles = "Admin")]
public class TransactionController : Controller
{
    private readonly IAdminPaymentService _adminPaymentService;

    public TransactionController(IAdminPaymentService adminPaymentService)
    {
        _adminPaymentService = adminPaymentService;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? serviceId, string? keyword, string? sortBy, int page = 1)
    {
        var vm = await _adminPaymentService.GetTransactionsAsync(from, to, serviceId, keyword, sortBy, page);
        
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");
        ViewBag.ServiceId = serviceId;
        ViewBag.Keyword = keyword;
        ViewBag.SortBy = sortBy;

        return View("~/Views/Admin/AdminTransaction/Index.cshtml", vm);
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var vm = await _adminPaymentService.GetTransactionDetailAsync(id);
        if (vm == null) return NotFound();

        return View("~/Views/Admin/AdminTransaction/Details.cshtml", vm);
    }
}

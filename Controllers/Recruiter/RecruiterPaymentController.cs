using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevHub.Controllers.Recruiter;

[Authorize(Roles = "RECRUITER")]
[Route("Recruiter/Payment")]
public class RecruiterPaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IRecruiterRepository _recruiterRepo;

    public RecruiterPaymentController(IPaymentService paymentService, IRecruiterRepository recruiterRepo)
    {
        _paymentService = paymentService;
        _recruiterRepo = recruiterRepo;
    }

    private int GetRecruiterId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
    
    private async Task<int?> GetCompanyIdAsync()
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(GetRecruiterId());
        return recruiter?.CompanyId;
    }

    [HttpGet("Subscription")]
    public async Task<IActionResult> Subscription()
    {
        var companyId = await GetCompanyIdAsync();
        if (companyId == null)
        {
            TempData["ErrorMessage"] = "Vui lòng cập nhật thông tin công ty trước khi mua gói dịch vụ.";
            return Redirect("/Recruiter/Settings?tab=company");
        }
        var vm = await _paymentService.GetSubscriptionPageAsync(companyId.Value);
        ViewData["ActiveMenu"] = "Services";
        return View("~/Views/Recruiter/Payment/Subscription.cshtml", vm);
    }

    [HttpGet("CheckVoucher")]
    public async Task<IActionResult> CheckVoucher(int serviceId, string code)
    {
        var companyId = await GetCompanyIdAsync();
        if (companyId == null) return Json(new { IsValid = false, Message = "Chưa có công ty." });
        
        var result = await _paymentService.ValidateVoucherAsync(companyId.Value, serviceId, code);
        return Json(result);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePaymentRequestVm req)
    {
        var companyId = await GetCompanyIdAsync();
        if (companyId == null)
        {
            TempData["ErrorMessage"] = "Vui lòng cập nhật thông tin công ty.";
            return Redirect("/Recruiter/Settings?tab=company");
        }
        
        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var result = await _paymentService.CreatePaymentUrlAsync(companyId.Value, req, clientIp);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction("Subscription");
        }

        if (result.IsFreeUpgrade)
        {
            TempData["SuccessMessage"] = "Nâng cấp gói miễn phí thành công!";
            return RedirectToAction("History");
        }

        return Redirect(result.Url!);
    }

    [HttpGet("VnpayReturn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnpayReturn()
    {
        var result = await _paymentService.ConfirmAsync(Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));

        if (result.Success)
        {
            TempData["SuccessMessage"] = "Thanh toán thành công!";
        }
        else
        {
            TempData["ErrorMessage"] = $"Thanh toán thất bại: {result.Message}";
        }

        // Return a view instead of direct redirect so the user sees the VNPay result
        ViewBag.Message = result.Message;
        ViewBag.Success = result.Success;
        return View("~/Views/Recruiter/Payment/VnpayReturn.cshtml");
    }

    [HttpGet("/api/recruiter/payment/vnpay-ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnpayIpn()
    {
        var result = await _paymentService.ConfirmAsync(Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));
        return Json(new { RspCode = result.ResponseCode, Message = result.Message });
    }

    [HttpGet("History")]
    public async Task<IActionResult> History(DateTime? from, DateTime? to, int? serviceId, int page = 1)
    {
        var companyId = await GetCompanyIdAsync();
        if (companyId == null)
        {
            TempData["ErrorMessage"] = "Vui lòng cập nhật thông tin công ty.";
            return Redirect("/Recruiter/Settings?tab=company");
        }
        var vm = await _paymentService.GetHistoryAsync(companyId.Value, from, to, serviceId, page);
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");
        ViewBag.ServiceId = serviceId;
        ViewData["ActiveMenu"] = "Services";
        return View("~/Views/Recruiter/Payment/History.cshtml", vm);
    }

    [HttpGet("HistoryDetails/{id}")]
    public async Task<IActionResult> HistoryDetails(int id)
    {
        var companyId = await GetCompanyIdAsync();
        if (companyId == null) return NotFound();
        
        var vm = await _paymentService.GetHistoryDetailAsync(companyId.Value, id);
        if (vm == null) return NotFound();
        ViewData["ActiveMenu"] = "Services";
        return View("~/Views/Recruiter/Payment/HistoryDetails.cshtml", vm);
    }

    [HttpGet("HistoryExport")]
    public async Task<IActionResult> HistoryExport(DateTime? from, DateTime? to, int? serviceId)
    {
        var companyId = await GetCompanyIdAsync();
        if (companyId == null) return NotFound();
        
        var excelBytes = await _paymentService.ExportHistoryAsync(companyId.Value, from, to, serviceId);
        string fileName = $"PaymentHistory_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

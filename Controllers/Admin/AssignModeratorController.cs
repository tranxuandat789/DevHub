using DevHub.Data;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevHub.Controllers.Admin;

[Authorize(AuthenticationSchemes = "AdminCookies", Roles = "Admin")]
[Route("AssignModerator")]
public class AssignModeratorController : Controller
{
    private readonly IAssignModeratorService _assignService;
    private readonly ItrecruitmentDbContext   _context;

    public AssignModeratorController(
        IAssignModeratorService assignService,
        ItrecruitmentDbContext context)
    {
        _assignService = assignService;
        _context       = context;
    }

    // -----------------------------------------------------------------------
    // GET /AssignModerator
    // -----------------------------------------------------------------------
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? filterIndustryCompany  = null,
        int?    filterServiceIdCompany = null,
        string? filterIndustryJob      = null,
        int?    filterServiceIdJob     = null,
        string? filterIndustryReview   = null)
    {
        var vm = new AssignModeratorViewModel
        {
            FilterIndustryCompany  = filterIndustryCompany,
            FilterServiceIdCompany = filterServiceIdCompany,
            FilterIndustryJob      = filterIndustryJob,
            FilterServiceIdJob     = filterServiceIdJob,
            FilterIndustryReview   = filterIndustryReview,

            // Stat cards
            PendingCompanies = await _assignService.GetUnassignedCountAsync("COMPANY_APPROVAL", filterIndustryCompany, filterServiceIdCompany),
            PendingJobPosts  = await _assignService.GetUnassignedCountAsync("JOB_POST",  filterIndustryJob,  filterServiceIdJob),
            PendingReviews   = await _assignService.GetUnassignedCountAsync("REVIEW",    filterIndustryReview),

            // Workload per section
            CompanyModerators = await _assignService.GetWorkloadByTaskTypeAsync("COMPANY_APPROVAL"),
            JobPostModerators = await _assignService.GetWorkloadByTaskTypeAsync("JOB_POST"),
            ReviewModerators  = await _assignService.GetWorkloadByTaskTypeAsync("REVIEW"),

            // Filter options
            Industries      = await _assignService.GetIndustriesAsync(),
            ServicePackages = await _assignService.GetActivePackagesAsync()
        };

        return View("~/Views/Admin/AssignModerator/Index.cshtml", vm);
    }

    // -----------------------------------------------------------------------
    // POST /AssignModerator/Assign   — assign N records cho 1 mod
    // -----------------------------------------------------------------------
    [HttpPost("Assign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(AssignBatchRequest req)
    {
        int adminId = GetCurrentAdminId();

        int assigned = await _assignService.AssignToModeratorAsync(
            req.TaskType, req.ModeratorId, req.Count, adminId,
            req.FilterIndustry, req.FilterServiceId);

        if (assigned > 0)
            TempData["SuccessMsg"] = $"✅ Đã phân công {assigned} công việc [{TaskTypeLabel(req.TaskType)}] thành công!";
        else
            TempData["ErrorMsg"] = "Không có công việc nào phù hợp để phân công (có thể đã được assign hết).";

        return RedirectToAction(nameof(Index));
    }

    // -----------------------------------------------------------------------
    // POST /AssignModerator/AutoAssign  — chia đều tất cả
    // -----------------------------------------------------------------------
    [HttpPost("AutoAssign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAssign(
        string taskType,
        string? filterIndustry  = null,
        int?    filterServiceId = null)
    {
        int adminId = GetCurrentAdminId();

        int total = await _assignService.AutoAssignEvenlyAsync(
            taskType, adminId, filterIndustry, filterServiceId);

        if (total > 0)
            TempData["SuccessMsg"] = $"✅ Đã tự động phân chia đều {total} công việc [{TaskTypeLabel(taskType)}] cho các Moderator!";
        else
            TempData["ErrorMsg"] = "Không có công việc nào để phân chia, hoặc chưa có Moderator được gán loại công việc này.";

        return RedirectToAction(nameof(Index));
    }

    // -----------------------------------------------------------------------
    // GET /AssignModerator/GetStats  — AJAX: pending count sau khi filter
    // -----------------------------------------------------------------------
    [HttpGet("GetStats")]
    public async Task<IActionResult> GetStats(
        string taskType,
        string? filterIndustry  = null,
        int?    filterServiceId = null)
    {
        int count = await _assignService.GetUnassignedCountAsync(taskType, filterIndustry, filterServiceId);
        return Json(new { count });
    }

    // -----------------------------------------------------------------------
    // GET /AssignModerator/History
    // -----------------------------------------------------------------------
    [HttpGet("History")]
    public async Task<IActionResult> History(
        string? filterTaskType = null,
        string? filterDateFrom = null,
        string? filterDateTo   = null,
        int     page           = 1)
    {
        const int pageSize = 20;

        DateTime? fromDate = string.IsNullOrEmpty(filterDateFrom) ? null : DateTime.Parse(filterDateFrom);
        DateTime? toDate   = string.IsNullOrEmpty(filterDateTo)   ? null : DateTime.Parse(filterDateTo);

        var (items, total) = await _assignService.GetHistoryAsync(
            filterTaskType, fromDate, toDate, page, pageSize);

        var vm = new AssignHistoryViewModel
        {
            Items          = items,
            TotalCount     = total,
            Page           = page,
            PageSize       = pageSize,
            FilterTaskType = filterTaskType,
            FilterDateFrom = filterDateFrom,
            FilterDateTo   = filterDateTo
        };

        return View("~/Views/Admin/AssignModerator/History.cshtml", vm);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------
    private int GetCurrentAdminId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out int id) ? id : 0;
    }

    private static string TaskTypeLabel(string taskType) => taskType switch
    {
        "COMPANY_APPROVAL" => "Duyệt công ty",
        "JOB_POST"         => "Duyệt bài đăng",
        "REVIEW"           => "Duyệt đánh giá",
        _                  => taskType
    };
}

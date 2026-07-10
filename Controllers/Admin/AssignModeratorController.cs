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
    private readonly DevHub.Repositories.Interfaces.IIndustryAssignmentRepository _industryRepo;
    private readonly DevHub.Repositories.Interfaces.ICompanyRepository _companyRepo;

    public AssignModeratorController(
        IAssignModeratorService assignService,
        ItrecruitmentDbContext context,
        DevHub.Repositories.Interfaces.IIndustryAssignmentRepository industryRepo,
        DevHub.Repositories.Interfaces.ICompanyRepository companyRepo)
    {
        _assignService = assignService;
        _context       = context;
        _industryRepo  = industryRepo;
        _companyRepo   = companyRepo;
    }

    // -----------------------------------------------------------------------
    // GET /AssignModerator
    // -----------------------------------------------------------------------
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // 1. Lấy danh sách ngành từ Company (Distinct)
        var allCompanies = await _companyRepo.GetAllAsync();
        var allIndustries = allCompanies
            .Where(c => !string.IsNullOrWhiteSpace(c.Industry))
            .Select(c => c.Industry.Trim())
            .Distinct()
            .OrderBy(i => i)
            .ToList();

        // 2. Lấy danh sách Moderator khả dụng cho từng loại
        var compMods = await _assignService.GetWorkloadByTaskTypeAsync("COMPANY_APPROVAL");
        var jobMods  = await _assignService.GetWorkloadByTaskTypeAsync("JOB_POST");
        var revMods  = await _assignService.GetWorkloadByTaskTypeAsync("REVIEW");

        // 3. Lấy phân công hiện tại
        var compAssignments = await _industryRepo.GetAllByTaskTypeAsync("COMPANY_APPROVAL");
        var jobAssignments  = await _industryRepo.GetAllByTaskTypeAsync("JOB_POST");
        var revAssignments  = await _industryRepo.GetAllByTaskTypeAsync("REVIEW");

        // Helper func mapping
        List<IndustryAssignmentItemDto> MapIndustries(List<DevHub.Models.ModeratorIndustryAssignment> assignments)
        {
            return allIndustries.Select(ind => {
                var a = assignments.FirstOrDefault(x => x.Industry == ind);
                return new IndustryAssignmentItemDto
                {
                    IndustryName = ind,
                    AssignedModeratorId = a?.ModeratorId,
                    AssignedModeratorName = a?.Moderator?.FullName ?? a?.Moderator?.Username ?? ""
                };
            }).ToList();
        }

        var vm = new AssignModeratorViewModel
        {
            // Pending stats có thể giữ lại nếu muốn hiển thị (tuỳ chọn)
            PendingCompanies = await _assignService.GetUnassignedCountAsync("COMPANY_APPROVAL"),
            PendingJobPosts  = await _assignService.GetUnassignedCountAsync("JOB_POST"),
            PendingReviews   = await _assignService.GetUnassignedCountAsync("REVIEW"),

            CompanyModerators = compMods.Select(m => new ModeratorSimpleDto { ModeratorId = m.ModeratorId, FullName = m.FullName, Email = m.Email }).ToList(),
            JobPostModerators = jobMods.Select(m => new ModeratorSimpleDto { ModeratorId = m.ModeratorId, FullName = m.FullName, Email = m.Email }).ToList(),
            ReviewModerators  = revMods.Select(m => new ModeratorSimpleDto { ModeratorId = m.ModeratorId, FullName = m.FullName, Email = m.Email }).ToList(),

            CompanyIndustryAssignments = MapIndustries(compAssignments),
            JobPostIndustryAssignments = MapIndustries(jobAssignments),
            ReviewIndustryAssignments  = MapIndustries(revAssignments),
        };

        return View("~/Views/Admin/AssignModerator/Index.cshtml", vm);
    }

    // -----------------------------------------------------------------------
    // POST /AssignModerator/AssignIndustryBatch
    // -----------------------------------------------------------------------
    [HttpPost("AssignIndustryBatch")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignIndustryBatch([FromForm] string taskType, [FromForm] Dictionary<string, int?> assignments)
    {
        int adminId = GetCurrentAdminId();

        // 1. Lấy tất cả phân công hiện tại của taskType này
        var existing = await _context.ModeratorIndustryAssignments
            .Where(x => x.TaskType == taskType)
            .ToListAsync();

        // 2. Xóa hết phân công cũ để set lại theo batch
        _context.ModeratorIndustryAssignments.RemoveRange(existing);

        // 3. Thêm các phân công mới (những ngành có chọn Moderator)
        if (assignments != null)
        {
            foreach (var kvp in assignments)
            {
                if (kvp.Value.HasValue)
                {
                    _context.ModeratorIndustryAssignments.Add(new DevHub.Models.ModeratorIndustryAssignment
                    {
                        ModeratorId = kvp.Value.Value,
                        TaskType = taskType,
                        Industry = kvp.Key,
                        AssignedBy = adminId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }
        }

            await _context.SaveChangesAsync();

        TempData["SuccessMsg"] = $"Đã lưu phân công ngành cho {TaskTypeLabel(taskType)} thành công!";
        return RedirectToAction("Index");
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
    // GET /AssignModerator/SuggestDistribution
    // -----------------------------------------------------------------------
    [HttpGet("SuggestDistribution")]
    public async Task<IActionResult> SuggestDistribution([FromQuery] string taskType)
    {
        // 1. Lấy danh sách Moderator cho taskType
        var mods = await _assignService.GetWorkloadByTaskTypeAsync(taskType);
        if (mods == null || mods.Count == 0) return Json(new Dictionary<string, int>());

        // 2. Lấy số lượng Pending theo từng ngành
        var pendingCounts = await _assignService.GetPendingCountByIndustryAsync(taskType);

        // 3. Khởi tạo mảng theo dõi workload của mỗi Mod trong phiên chia này
        var modWorkloads = mods.ToDictionary(m => m.ModeratorId, m => 0);
        var result = new Dictionary<string, int?>();

        // 4. Lọc ra các ngành có việc (> 0) và sắp xếp giảm dần
        var activeIndustries = pendingCounts.Where(p => p.Value > 0).OrderByDescending(p => p.Value).ToList();
        
        foreach (var ind in activeIndustries)
        {
            // Tìm Mod đang có ít việc nhất hiện tại
            var minModId = modWorkloads.OrderBy(w => w.Value).First().Key;
            
            // Gán ngành này cho Mod đó
            result[ind.Key] = minModId;
            
            // Cộng dồn workload
            modWorkloads[minModId] += ind.Value;
        }

        // 5. Các ngành không có việc (0) thì chia đều (Round-Robin)
        var idleIndustries = pendingCounts.Where(p => p.Value == 0).OrderBy(p => p.Key).ToList();
        int modIndex = 0;
        var modIdsList = modWorkloads.Keys.ToList();
        
        foreach (var ind in idleIndustries)
        {
            if (modIdsList.Count > 0)
            {
                result[ind.Key] = modIdsList[modIndex % modIdsList.Count];
                modIndex++;
            }
        }

        return Json(result);
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

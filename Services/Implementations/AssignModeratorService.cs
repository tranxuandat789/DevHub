using DevHub.Data;
using DevHub.Models;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DevHub.Services.Implementations;

public class AssignModeratorService : IAssignModeratorService
{
    private readonly ItrecruitmentDbContext _context;
    private readonly DevHub.Repositories.Interfaces.IIndustryAssignmentRepository _industryRepo;

    public AssignModeratorService(ItrecruitmentDbContext context, DevHub.Repositories.Interfaces.IIndustryAssignmentRepository industryRepo)
    {
        _context = context;
        _industryRepo = industryRepo;
    }

    // ----------------------------------------------------------------
    // Task Type Management
    // ----------------------------------------------------------------

    public async Task<string?> GetTaskTypeAsync(int moderatorId)
    {
        var record = await _context.ModeratorTaskTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ModeratorId == moderatorId);
        return record?.TaskType;
    }

    public async Task<bool> SetTaskTypeAsync(int moderatorId, string taskType, int assignedByAdminId)
    {
        var existing = await _context.ModeratorTaskTypes
            .FirstOrDefaultAsync(m => m.ModeratorId == moderatorId);

        if (existing == null)
        {
            _context.ModeratorTaskTypes.Add(new ModeratorTaskType
            {
                ModeratorId = moderatorId,
                TaskType    = taskType,
                AssignedBy  = assignedByAdminId,
                CreatedAt   = DateTime.Now,
                UpdatedAt   = DateTime.Now
            });
        }
        else
        {
            existing.TaskType   = taskType;
            existing.AssignedBy = assignedByAdminId;
            existing.UpdatedAt  = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // ----------------------------------------------------------------
    // Workload & Stats
    // ----------------------------------------------------------------

    public async Task<List<ModeratorWorkloadDto>> GetWorkloadByTaskTypeAsync(string taskType)
    {
        var today     = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        // Lấy moderators thuộc task type này
        var moderators = await _context.ModeratorTaskTypes
            .AsNoTracking()
            .Where(mt => mt.TaskType == taskType)
            .Include(mt => mt.Moderator)
                .ThenInclude(a => a.AdminNavigation)
            .ToListAsync();

        var result = new List<ModeratorWorkloadDto>();

        foreach (var mt in moderators)
        {
            if (mt.Moderator?.AdminNavigation?.IsActive != true) continue;

            int assigned = 0, processedToday = 0, processedWeek = 0;

            if (taskType == "COMPANY_APPROVAL")
            {
                assigned       = await _context.Companies.CountAsync(c => c.ModeratorId == mt.ModeratorId && c.Status == "PENDING");
                processedToday = await _context.Companies.CountAsync(c => c.ModeratorId == mt.ModeratorId && c.Status != "PENDING" && c.Status != "REJECTED");
                processedWeek  = processedToday; // simplified — track by moderated_at nếu cần
            }
            else if (taskType == "JOB_POST")
            {
                assigned       = await _context.JobPosts.CountAsync(j => j.ModeratorId == mt.ModeratorId && j.Status == "PENDING");
                processedToday = await _context.JobPosts.CountAsync(j => j.ModeratorId == mt.ModeratorId && j.ApprovedAt >= today);
                processedWeek  = await _context.JobPosts.CountAsync(j => j.ModeratorId == mt.ModeratorId && j.ApprovedAt >= weekStart);
            }
            else if (taskType == "REVIEW")
            {
                assigned       = await _context.ReviewCompanies.CountAsync(r => r.ModeratorId == mt.ModeratorId && r.Status == "PENDING");
                processedToday = await _context.ReviewCompanies.CountAsync(r => r.ModeratorId == mt.ModeratorId && r.ModeratedAt >= today);
                processedWeek  = await _context.ReviewCompanies.CountAsync(r => r.ModeratorId == mt.ModeratorId && r.ModeratedAt >= weekStart);
            }

            int total = assigned + processedWeek;
            double rate = total > 0 ? Math.Round((double)processedWeek / total * 100, 1) : 0;

            result.Add(new ModeratorWorkloadDto
            {
                ModeratorId       = mt.ModeratorId,
                FullName          = mt.Moderator.FullName ?? mt.Moderator.Username,
                Email             = mt.Moderator.AdminNavigation?.Email ?? "",
                TaskType          = taskType,
                AssignedPending   = assigned,
                ProcessedToday    = processedToday,
                ProcessedThisWeek = processedWeek,
                CompletionRate    = rate
            });
        }

        return result.OrderBy(m => m.AssignedPending).ToList();
    }

    public async Task<int> GetUnassignedCountAsync(string taskType, string? filterIndustry = null, int? filterServiceId = null)
    {
        if (taskType == "COMPANY_APPROVAL")
        {
            var q = _context.Companies.Where(c => c.Status == "PENDING" && c.ModeratorId == null);
            if (!string.IsNullOrEmpty(filterIndustry)) q = q.Where(c => c.Industry == filterIndustry);
            if (filterServiceId.HasValue)
                q = q.Where(c => c.CompanyPackageHistories.Any(h => h.ServiceId == filterServiceId && h.IsActive == true));
            return await q.CountAsync();
        }
        else if (taskType == "JOB_POST")
        {
            var q = _context.JobPosts.Where(j => j.Status == "PENDING" && j.ModeratorId == null);
            if (!string.IsNullOrEmpty(filterIndustry)) q = q.Where(j => j.Company.Industry == filterIndustry);
            if (filterServiceId.HasValue)
                q = q.Where(j => j.CompanyPackageHistoryId != null &&
                    j.CompanyPackageHistory!.ServiceId == filterServiceId);
            return await q.CountAsync();
        }
        else if (taskType == "REVIEW")
        {
            var q = _context.ReviewCompanies.Where(r => r.Status == "PENDING" && r.ModeratorId == null);
            if (!string.IsNullOrEmpty(filterIndustry)) q = q.Where(r => r.Company.Industry == filterIndustry);
            return await q.CountAsync();
        }
        return 0;
    }

    public async Task<Dictionary<string, int>> GetPendingCountByIndustryAsync(string taskType)
    {
        var result = new Dictionary<string, int>();

        if (taskType == "COMPANY_APPROVAL")
        {
            var counts = await _context.Companies
                .Where(c => c.Status == "PENDING" && c.Industry != null)
                .GroupBy(c => c.Industry!)
                .Select(g => new { Industry = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var item in counts) result[item.Industry] = item.Count;
        }
        else if (taskType == "JOB_POST")
        {
            var counts = await _context.JobPosts
                .Include(j => j.Company)
                .Where(j => j.Status == "PENDING" && j.Company.Industry != null)
                .GroupBy(j => j.Company.Industry!)
                .Select(g => new { Industry = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var item in counts) result[item.Industry] = item.Count;
        }
        else if (taskType == "REVIEW")
        {
            var counts = await _context.ReviewCompanies
                .Include(r => r.Company)
                .Where(r => r.Status == "PENDING" && r.Company.Industry != null)
                .GroupBy(r => r.Company.Industry!)
                .Select(g => new { Industry = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var item in counts) result[item.Industry] = item.Count;
        }

        // Đảm bảo các ngành không có pending vẫn có key trong dictionary (bằng 0)
        var allIndustries = await GetIndustriesAsync();
        foreach (var ind in allIndustries)
        {
            if (!result.ContainsKey(ind))
            {
                result[ind] = 0;
            }
        }

        return result;
    }

    // ----------------------------------------------------------------
    // Assign Operations
    // ----------------------------------------------------------------

    public async Task<int> AssignToModeratorAsync(
        string taskType, int moderatorId, int count, int adminId,
        string? filterIndustry = null, int? filterServiceId = null)
    {
        int assigned = 0;

        if (taskType == "COMPANY_APPROVAL")
        {
            var q = _context.Companies
                .Where(c => c.Status == "PENDING" && c.ModeratorId == null);
            if (!string.IsNullOrEmpty(filterIndustry)) q = q.Where(c => c.Industry == filterIndustry);
            if (filterServiceId.HasValue)
                q = q.Where(c => c.CompanyPackageHistories.Any(h => h.ServiceId == filterServiceId && h.IsActive == true));

            assigned = await q.Take(count)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.ModeratorId, moderatorId));
        }
        else if (taskType == "JOB_POST")
        {
            var q = _context.JobPosts
                .Where(j => j.Status == "PENDING" && j.ModeratorId == null);
            if (!string.IsNullOrEmpty(filterIndustry)) q = q.Where(j => j.Company.Industry == filterIndustry);
            if (filterServiceId.HasValue)
                q = q.Where(j => j.CompanyPackageHistoryId != null &&
                    j.CompanyPackageHistory!.ServiceId == filterServiceId);

            assigned = await q.Take(count)
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.ModeratorId, moderatorId));
        }
        else if (taskType == "REVIEW")
        {
            var q = _context.ReviewCompanies
                .Where(r => r.Status == "PENDING" && r.ModeratorId == null);
            if (!string.IsNullOrEmpty(filterIndustry)) q = q.Where(r => r.Company.Industry == filterIndustry);

            assigned = await q.Take(count)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.ModeratorId, moderatorId));
        }

        // Ghi audit log
        if (assigned > 0)
            await WriteAuditLogAsync(adminId, taskType, moderatorId, assigned, filterIndustry, filterServiceId);

        return assigned;
    }

    public async Task<int> AutoAssignEvenlyAsync(
        string taskType, int adminId,
        string? filterIndustry = null, int? filterServiceId = null)
    {
        // Lấy danh sách mod active của task type
        var modIds = await _context.ModeratorTaskTypes
            .AsNoTracking()
            .Where(mt => mt.TaskType == taskType && mt.Moderator.AdminNavigation!.IsActive == true)
            .Select(mt => mt.ModeratorId)
            .ToListAsync();

        if (modIds.Count == 0) return 0;

        int total = await GetUnassignedCountAsync(taskType, filterIndustry, filterServiceId);
        if (total == 0) return 0;

        int baseCount   = total / modIds.Count;
        int remainder   = total % modIds.Count;
        int totalAssigned = 0;

        for (int i = 0; i < modIds.Count; i++)
        {
            int count = baseCount + (i == 0 ? remainder : 0); // mod đầu nhận phần dư
            if (count <= 0) continue;

            int done = await AssignToModeratorAsync(
                taskType, modIds[i], count, adminId, filterIndustry, filterServiceId);
            totalAssigned += done;
        }

        return totalAssigned;
    }

    /// <summary>
    /// Gán record mới vừa tạo vào moderator ít việc nhất (auto-assign realtime)
    /// Gọi từ service tạo Company / JobPost / Review
    /// </summary>
    public async Task AutoAssignNewRecordAsync(string taskType, int recordId)
    {
        // 1. Xác định industry của record
        string? industry = taskType switch {
            "COMPANY_APPROVAL" => (await _context.Companies.FindAsync(recordId))?.Industry,
            "JOB_POST"         => (await _context.JobPosts
                                       .Include(j => j.Company)
                                       .FirstOrDefaultAsync(j => j.JobId == recordId))?.Company?.Industry,
            "REVIEW"           => (await _context.ReviewCompanies
                                       .Include(r => r.Company)
                                       .FirstOrDefaultAsync(r => r.ReviewId == recordId))?.Company?.Industry,
            _                  => null
        };

        // 2. Tìm Moderator theo (taskType, industry)
        int? moderatorId = null;
        if (!string.IsNullOrEmpty(industry))
            moderatorId = await _industryRepo.GetModeratorIdAsync(taskType, industry);

        // 3. Fallback: ít việc nhất trong cùng sub-role
        if (moderatorId == null)
        {
            var mods = await GetWorkloadByTaskTypeAsync(taskType);
            moderatorId = mods.OrderBy(m => m.AssignedPending).FirstOrDefault()?.ModeratorId;
        }

        if (moderatorId == null) return;

        // 4. Gán vào record
        int assignedCount = 0;
        if (taskType == "COMPANY_APPROVAL")
            assignedCount = await _context.Companies.Where(c => c.CompanyId == recordId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.ModeratorId, moderatorId));
        else if (taskType == "JOB_POST")
            assignedCount = await _context.JobPosts.Where(j => j.JobId == recordId)
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.ModeratorId, moderatorId));
        else if (taskType == "REVIEW")
            assignedCount = await _context.ReviewCompanies.Where(r => r.ReviewId == recordId)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.ModeratorId, moderatorId));

        if (assignedCount > 0)
        {
            await WriteAuditLogAsync(null, taskType, moderatorId.Value, 1, industry, null);
        }
    }

    // ----------------------------------------------------------------
    // History
    // ----------------------------------------------------------------

    public async Task<(List<AssignHistoryItemDto> Items, int TotalCount)> GetHistoryAsync(
        string? filterTaskType, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
    {
        var q = _context.AuditLogs
            .AsNoTracking()
            .Where(l => l.Action == "ASSIGN_MODERATOR");

        if (!string.IsNullOrEmpty(filterTaskType))
            q = q.Where(l => l.EntityType == filterTaskType);

        if (fromDate.HasValue)
            q = q.Where(l => l.CreatedAt >= fromDate);

        if (toDate.HasValue)
            q = q.Where(l => l.CreatedAt <= toDate.Value.AddDays(1));

        int total = await q.CountAsync();
        var logs  = await q.OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map log entries
        var adminIds = logs.Select(l => l.UserId).Where(id => id.HasValue).Distinct().ToList();
        var admins   = await _context.Admins.AsNoTracking()
            .Where(a => adminIds.Contains(a.AdminId))
            .ToDictionaryAsync(a => a.AdminId, a => a.FullName ?? a.Username);

        var modIdsInLogs = new List<int>();
        var parsedLogs = logs.Select(l =>
        {
            int modId = 0; int count = 0; string filter = "";
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(l.NewValue ?? "{}");
                if (data != null)
                {
                    data.TryGetValue("moderator_id", out var mId);
                    data.TryGetValue("count", out var cnt);
                    data.TryGetValue("filter", out var flt);
                    modId  = mId.ValueKind != JsonValueKind.Undefined ? mId.GetInt32() : 0;
                    count  = cnt.ValueKind != JsonValueKind.Undefined ? cnt.GetInt32() : 0;
                    filter = flt.ValueKind != JsonValueKind.Undefined ? flt.ToString() : "";
                }
            }
            catch { }
            return (Log: l, ModId: modId, Count: count, Filter: filter);
        }).ToList();

        var uniqueModIds = parsedLogs.Select(x => x.ModId).Distinct().ToList();
        var mods = await _context.Admins.AsNoTracking()
            .Where(a => uniqueModIds.Contains(a.AdminId))
            .ToDictionaryAsync(a => a.AdminId, a => a.FullName ?? a.Username);

        var items = parsedLogs.Select(x => new AssignHistoryItemDto
        {
            LogId         = x.Log.LogId,
            AdminName     = x.Log.UserId.HasValue 
                                ? (admins.ContainsKey(x.Log.UserId.Value) ? admins[x.Log.UserId.Value] : "Unknown Admin") 
                                : "Hệ thống tự động",
            TaskType      = x.Log.EntityType ?? "",
            ModeratorName = mods.ContainsKey(x.ModId) ? mods[x.ModId] : "Unknown",
            RecordCount   = x.Count,
            FilterUsed    = x.Filter,
            AssignedAt    = x.Log.CreatedAt
        }).ToList();

        return (items, total);
    }

    // ----------------------------------------------------------------
    // Filter Options
    // ----------------------------------------------------------------

    public async Task<List<string>> GetIndustriesAsync()
        => await _context.Companies
            .AsNoTracking()
            .Where(c => c.Industry != null)
            .Select(c => c.Industry!)
            .Distinct()
            .OrderBy(i => i)
            .ToListAsync();

    public async Task<List<ServicePackageSimpleDto>> GetActivePackagesAsync()
        => await _context.ServicePackages
            .AsNoTracking()
            .Where(s => s.IsActive == true)
            .Select(s => new ServicePackageSimpleDto
            {
                ServiceId   = s.ServiceId,
                PackageName = s.PackageName,
                Title       = s.Title
            })
            .ToListAsync();

    // ----------------------------------------------------------------
    // Private Helpers
    // ----------------------------------------------------------------

    private async Task WriteAuditLogAsync(
        int? adminId, string taskType, int moderatorId, int count,
        string? filterIndustry, int? filterServiceId)
    {
        var filterInfo = new { industry = filterIndustry, serviceId = filterServiceId };
        _context.AuditLogs.Add(new AuditLog
        {
            UserId     = adminId,
            UserType   = adminId.HasValue ? "ADMIN" : "SYSTEM",
            Action     = "ASSIGN_MODERATOR",
            EntityType = taskType,
            EntityId   = null,
            NewValue   = JsonSerializer.Serialize(new
            {
                moderator_id = moderatorId,
                count        = count,
                filter       = JsonSerializer.Serialize(filterInfo)
            }),
            CreatedAt  = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }
}

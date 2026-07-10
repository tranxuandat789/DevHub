using DevHub.ViewModels.Admin;

namespace DevHub.Services.Interfaces;

public interface IAssignModeratorService
{
    // ---- Task Type Management ----

    /// <summary>Lấy task type hiện tại của moderator</summary>
    Task<string?> GetTaskTypeAsync(int moderatorId);

    /// <summary>Gán hoặc cập nhật task type cho moderator</summary>
    Task<bool> SetTaskTypeAsync(int moderatorId, string taskType, int assignedByAdminId);

    // ---- Workload & Stats ----

    /// <summary>Lấy workload của tất cả moderator thuộc 1 task type</summary>
    Task<List<ModeratorWorkloadDto>> GetWorkloadByTaskTypeAsync(string taskType);

    /// <summary>Số records PENDING chưa được assign (moderator_id IS NULL)</summary>
    Task<int> GetUnassignedCountAsync(string taskType, string? filterIndustry = null, int? filterServiceId = null);

    // ---- Assign Operations ----

    /// <summary>
    /// Assign N records cho 1 moderator cụ thể (có thể filter)
    /// Trả về số records thực sự được assign
    /// </summary>
    Task<int> AssignToModeratorAsync(
        string taskType,
        int moderatorId,
        int count,
        int adminId,
        string? filterIndustry = null,
        int? filterServiceId = null);

    /// <summary>
    /// Auto-assign đều tất cả records PENDING cho tất cả moderators của task type đó (round-robin)
    /// Trả về tổng số records đã assign
    /// </summary>
    Task<int> AutoAssignEvenlyAsync(
        string taskType,
        int adminId,
        string? filterIndustry = null,
        int? filterServiceId = null);

    /// <summary>
    /// Auto-assign record mới khi nó được tạo vào hệ thống
    /// (gọi từ service tạo record để gán ngay cho mod ít việc nhất)
    /// </summary>
    Task<int?> AutoAssignNewRecordAsync(string taskType, int recordId);

    // ---- History ----

    /// <summary>Lấy lịch sử assign từ audit_log</summary>
    Task<(List<AssignHistoryItemDto> Items, int TotalCount)> GetHistoryAsync(
        string? filterTaskType,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize);

    // ---- Filter Options ----

    /// <summary>Lấy danh sách industries từ DB để hiển thị dropdown</summary>
    Task<List<string>> GetIndustriesAsync();

    /// <summary>Lấy danh sách gói dịch vụ active</summary>
    Task<List<ServicePackageSimpleDto>> GetActivePackagesAsync();
}

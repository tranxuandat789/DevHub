namespace DevHub.ViewModels.Admin;

/// <summary>Workload của 1 moderator</summary>
public class ModeratorWorkloadDto
{
    public int    ModeratorId       { get; set; }
    public string FullName          { get; set; } = "";
    public string Email             { get; set; } = "";
    public string TaskType          { get; set; } = "";
    public int    AssignedPending   { get; set; }  // records pending được giao
    public int    ProcessedToday    { get; set; }  // đã xử lý hôm nay
    public int    ProcessedThisWeek { get; set; }  // đã xử lý tuần này
    public double CompletionRate    { get; set; }  // % hoàn thành (processed / (assigned+processed))
}

/// <summary>Lịch sử 1 batch assign</summary>
public class AssignHistoryItemDto
{
    public int       LogId        { get; set; }
    public string    AdminName    { get; set; } = "";
    public string    TaskType     { get; set; } = "";
    public string    ModeratorName{ get; set; } = "";
    public int       RecordCount  { get; set; }
    public string?   FilterUsed   { get; set; }
    public DateTime? AssignedAt   { get; set; }
}

/// <summary>Thông tin cơ bản của Moderator cho Dropdown</summary>
public class ModeratorSimpleDto
{
    public int    ModeratorId { get; set; }
    public string FullName    { get; set; } = "";
    public string Email       { get; set; } = "";
}

/// <summary>Thông tin một ngành và người đang phụ trách</summary>
public class IndustryAssignmentItemDto
{
    public string IndustryName { get; set; } = "";
    public int?   AssignedModeratorId { get; set; }
    public string AssignedModeratorName { get; set; } = "";
}

/// <summary>ViewModel chính cho trang /AssignModerator</summary>
public class AssignModeratorViewModel
{
    // Stat cards - số lượng pending (nếu vẫn muốn giữ)
    public int PendingCompanies { get; set; }
    public int PendingJobPosts  { get; set; }
    public int PendingReviews   { get; set; }

    // Danh sách Moderators khả dụng cho từng loại (để đổ vào dropdown)
    public List<ModeratorSimpleDto> CompanyModerators  { get; set; } = new();
    public List<ModeratorSimpleDto> JobPostModerators  { get; set; } = new();
    public List<ModeratorSimpleDto> ReviewModerators   { get; set; } = new();

    // Danh sách phân công ngành cho từng loại
    public List<IndustryAssignmentItemDto> CompanyIndustryAssignments { get; set; } = new();
    public List<IndustryAssignmentItemDto> JobPostIndustryAssignments { get; set; } = new();
    public List<IndustryAssignmentItemDto> ReviewIndustryAssignments  { get; set; } = new();
}

public class ServicePackageSimpleDto
{
    public int    ServiceId   { get; set; }
    public string PackageName { get; set; } = "";
    public string Title       { get; set; } = "";
}

/// <summary>ViewModel cho trang /AssignModerator/History</summary>
public class AssignHistoryViewModel
{
    public List<AssignHistoryItemDto> Items      { get; set; } = new();
    public int                        TotalCount { get; set; }
    public int                        Page       { get; set; }
    public int                        PageSize   { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // Filter
    public string? FilterTaskType { get; set; }
    public string? FilterDateFrom { get; set; }
    public string? FilterDateTo   { get; set; }
}

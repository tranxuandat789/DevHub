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

/// <summary>ViewModel chính cho trang /AssignModerator</summary>
public class AssignModeratorViewModel
{
    // Stat cards
    public int PendingCompanies { get; set; }
    public int PendingJobPosts  { get; set; }
    public int PendingReviews   { get; set; }

    // Moderators theo từng loại task
    public List<ModeratorWorkloadDto> CompanyModerators  { get; set; } = new();
    public List<ModeratorWorkloadDto> JobPostModerators  { get; set; } = new();
    public List<ModeratorWorkloadDto> ReviewModerators   { get; set; } = new();

    // Filter options
    public List<string>              Industries      { get; set; } = new();
    public List<ServicePackageSimpleDto> ServicePackages { get; set; } = new();

    // Filter giá trị từ form
    public string? FilterIndustryCompany  { get; set; }
    public int?    FilterServiceIdCompany { get; set; }
    public string? FilterIndustryJob      { get; set; }
    public int?    FilterServiceIdJob     { get; set; }
    public string? FilterIndustryReview   { get; set; }
}

public class ServicePackageSimpleDto
{
    public int    ServiceId   { get; set; }
    public string PackageName { get; set; } = "";
    public string Title       { get; set; } = "";
}

/// <summary>ViewModel cho form assign 1 batch</summary>
public class AssignBatchRequest
{
    public string TaskType     { get; set; } = "";  // COMPANY_APPROVAL | JOB_POST | REVIEW
    public int    ModeratorId  { get; set; }
    public int    Count        { get; set; }        // số lượng records muốn assign
    public string? FilterIndustry  { get; set; }
    public int?   FilterServiceId  { get; set; }
    public bool   AutoAssign   { get; set; }        // true = chia đều tất cả cho tất cả mods
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

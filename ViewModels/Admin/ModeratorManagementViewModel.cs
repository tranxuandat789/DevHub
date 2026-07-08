// 04/06/2026-DatTX
namespace DevHub.ViewModels.Admin;

// DTO for each row in the moderator table
public class ModeratorListItemDto
{
    public int     AdminId   { get; set; }
    public string  Username  { get; set; } = "";
    public string  FullName  { get; set; } = "";
    public string  Email     { get; set; } = "";
    public bool    IsActive  { get; set; }
    public DateTime? CreatedAt { get; set; }
    /// <summary>Loại task được assign: COMPANY_APPROVAL | JOB_POST | REVIEW | null nếu chưa set</summary>
    public string? TaskType  { get; set; }
}

// ViewModel for the Index page (list + pagination + search state)
public class ModeratorListViewModel
{
    public List<ModeratorListItemDto> Items      { get; set; } = new();
    public int                        TotalCount { get; set; }
    public int                        Page       { get; set; }
    public int                        PageSize   { get; set; } = 5;
    public string?                    Search     { get; set; }
    public string?                    Filter     { get; set; }

    // Total pages for pagination control
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

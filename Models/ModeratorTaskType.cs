using System;

namespace DevHub.Models;

public partial class ModeratorTaskType
{
    public int Id { get; set; }

    public int ModeratorId { get; set; }

    /// <summary>
    /// Loại công việc: COMPANY_APPROVAL | JOB_POST | REVIEW
    /// </summary>
    public string TaskType { get; set; } = null!;

    public int AssignedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Admin Moderator { get; set; } = null!;

    public virtual Admin AssignedByAdmin { get; set; } = null!;
}

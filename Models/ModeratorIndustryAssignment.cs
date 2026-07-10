using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class ModeratorIndustryAssignment
{
    public int Id { get; set; }
    public int ModeratorId { get; set; }
    public string TaskType { get; set; } = null!;
    public string Industry { get; set; } = null!;
    public int AssignedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual Admin Moderator { get; set; } = null!;
    public virtual Admin AssignedByAdmin { get; set; } = null!;
}

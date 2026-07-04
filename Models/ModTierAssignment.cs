using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class ModTierAssignment
{
    public int AssignmentId { get; set; }
    public int ModeratorId { get; set; }
    public int ServiceId { get; set; }
    public DateTime? CreatedAt { get; set; }

    public virtual Admin Moderator { get; set; } = null!;
    public virtual ServicePackage Service { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class CommonJobPosition
{
    public int PositionId { get; set; }

    public string PositionName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}

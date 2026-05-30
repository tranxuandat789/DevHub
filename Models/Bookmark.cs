using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Bookmark
{
    public int BookmarkId { get; set; }

    public int CandidateId { get; set; }

    public int JobId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual JobPost Job { get; set; } = null!;
}

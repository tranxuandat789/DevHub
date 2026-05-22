using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class ReviewRecruiter
{
    public int ReviewId { get; set; }

    public int CandidateId { get; set; }

    public int RecruiterId { get; set; }

    public int Rating { get; set; }

    public string Pros { get; set; } = null!;

    public string Cons { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsAnonymous { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Recruiter Recruiter { get; set; } = null!;
}

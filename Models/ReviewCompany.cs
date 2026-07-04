using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class ReviewCompany
{
    public int ReviewId { get; set; }

    public int CandidateId { get; set; }

    public int CompanyId { get; set; }

    public int Rating { get; set; }

    public string Pros { get; set; } = null!;

    public string Cons { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsAnonymous { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Status { get; set; } = "pending"; 

    public string? RejectionReason { get; set; }

    public int? ModeratorId { get; set; } 

    public DateTime? ModeratedAt { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual Admin? Moderator { get; set; }
}
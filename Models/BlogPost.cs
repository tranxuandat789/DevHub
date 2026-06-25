using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class BlogPost
{
    public int BlogId { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Author { get; set; }
    public int? AuthorId { get; set; }
    public int? PublisherId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? ApproverId { get; set; }
    public bool? IsDeleted { get; set; }
    public string? RejectReason { get; set; }
    public DateTime? RejectedAt { get; set; }
    public int? Status { get; set; }
    public string? Tags { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Admin? Publisher { get; set; }
    public virtual Recruiter? AuthorRecruiter { get; set; }
    public virtual Admin? Approver { get; set; }
}

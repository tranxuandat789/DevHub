using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevHub.Models;

public partial class Article
{
    public int ArticleId { get; set; }
    public int? CompanyId { get; set; }
    public string? Title { get; set; }
    public string Slug { get; set; } = null!;
    public string? Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? ApproverId { get; set; }
    public string? RejectReason { get; set; }

    [ForeignKey("ApproverId")]
    [InverseProperty("Articles")]
    public virtual Admin? Approver { get; set; }
    
    [ForeignKey("CompanyId")]
    [InverseProperty("Articles")]
    public virtual Company? Company { get; set; }


}

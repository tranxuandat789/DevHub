using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DevHub.Models;

public partial class JobPost
{
    public int JobId { get; set; }

    public int RecruiterId { get; set; }

    public string Title { get; set; } = null!;

    public int PositionId { get; set; }

    public string? Skill { get; set; }

    public string WorkingModel { get; set; } = null!;

    // How salary should be interpreted/displayed: RANGE | FROM | UPTO | NEGOTIABLE.
    public string SalaryType { get; set; } = "RANGE";

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? ExperienceLevel { get; set; }

    public string? Description { get; set; }

    public string? Requirement { get; set; }

    public string? Benefit { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Status { get; set; }

    public int? PriorityScore { get; set; }

    public int? HiringQuota { get; set; }

    public int? ApplicationCount { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int? ModeratorId { get; set; }
    //    public int RecruiterPackageHistoryId { get; set; --> to
    public int? RecruiterPackageHistoryId { get; set; }

    public string? RejectedReason { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual Admin? Moderator { get; set; }

    public virtual Recruiter Recruiter { get; set; } = null!;

    // public virtual RecruiterPackageHistory RecruiterPackageHistory { get; set; } --> to 
    public virtual RecruiterPackageHistory? RecruiterPackageHistory { get; set; }

    public virtual CommonJobPosition Position { get; set; } = null!;

    public virtual ICollection<CommonTechnology> Teches { get; set; } = new List<CommonTechnology>();

    // Provinces this job targets (job_post_province junction). Replaces the old
    // free-text `location` column.
    public virtual ICollection<Province> Provinces { get; set; } = new List<Province>();

    // Convenience display string built from the linked provinces so existing
    // read-only callers (`@job.Location`, JSON, in-memory VM projections) keep
    // working. NOT mapped to the database — requires Provinces to be loaded.
    [NotMapped]
    public string Location =>
        Provinces != null && Provinces.Count > 0
            ? string.Join(", ", Provinces.Select(p => p.ProvinceName))
            : string.Empty;
}
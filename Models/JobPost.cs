using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class JobPost
{
    public int JobId { get; set; }

    public int RecruiterId { get; set; }

    public string Title { get; set; } = null!;

    public int PositionId { get; set; }

    public string Location { get; set; } = null!;

    public string? Skill { get; set; }

    public string WorkingModel { get; set; } = null!;

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? ExperienceLevel { get; set; }

    public string? Description { get; set; }

    public string? Requirement { get; set; }

    public string? Benefit { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Status { get; set; }

    public bool? IsPromoted { get; set; }

    public int? PriorityScore { get; set; }

    public int? HiringQuota { get; set; }

    public int? ApplicationCount { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int? ModeratorId { get; set; }

    public int RecruiterPackageHistoryId { get; set; }

    public string? RejectedReason { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual Admin? Moderator { get; set; }

    public virtual Recruiter Recruiter { get; set; } = null!;

    public virtual RecruiterPackageHistory RecruiterPackageHistory { get; set; } = null!;

    public virtual CommonJobPosition Position { get; set; } = null!;

    public virtual ICollection<CommonTechnology> Teches { get; set; } = new List<CommonTechnology>();
}
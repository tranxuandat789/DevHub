using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Interview
{
    public int InterviewId { get; set; }

    public int ApplicationId { get; set; }

    public int RecruiterId { get; set; }

    public int CandidateId { get; set; }

    public DateTime ScheduledTime { get; set; }

    public string? MeetingLink { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public string? InterviewType { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Recruiter Recruiter { get; set; } = null!;
}

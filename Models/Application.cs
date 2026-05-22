using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Application
{
    public int ApplicationId { get; set; }

    public int CandidateId { get; set; }

    public int CvId { get; set; }

    public int JobId { get; set; }

    public string? CoverLetter { get; set; }

    public DateTime? AppliedAt { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Cv Cv { get; set; } = null!;

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual JobPost Job { get; set; } = null!;
}

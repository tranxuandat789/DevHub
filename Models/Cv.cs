using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Cv
{
    public int CvId { get; set; }

    public int CandidateId { get; set; }

    public string Title { get; set; } = null!;

    public string? Education { get; set; }

    public string? Experience { get; set; }

    public string? Skills { get; set; }

    public string? Languages { get; set; }

    public string? CvUrl { get; set; }

    public bool? IsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Candidate Candidate { get; set; } = null!;
}

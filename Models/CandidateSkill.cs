using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class CandidateSkill
{
    public int CandidateId { get; set; }

    public int TechId { get; set; }

    public string? Level { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual CommonTechnology Tech { get; set; } = null!;
}

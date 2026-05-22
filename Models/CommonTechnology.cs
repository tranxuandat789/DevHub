using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class CommonTechnology
{
    public int TechId { get; set; }

    public string TechName { get; set; } = null!;

    public string? Category { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

    public virtual ICollection<JobPost> Jobs { get; set; } = new List<JobPost>();
}

using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Recruiter
{
    public int RecruiterId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public string? Phone { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsCompanyAdmin { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual UserAccount RecruiterNavigation { get; set; } = null!;

    public virtual Company? Company { get; set; }
}

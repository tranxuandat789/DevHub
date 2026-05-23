using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Candidate
{
    public int CandidateId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public decimal? ExpectedSalaryMin { get; set; }

    public decimal? ExpectedSalaryMax { get; set; }

    public string? PreferredLocation { get; set; }

    public int? ExperienceYears { get; set; }

    public bool? CvSearchable { get; set; }

    public int? ProfileCompletion { get; set; }

    public string? ImageUrl { get; set; }

    public string? SocialMediaUrl { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual UserAccount CandidateNavigation { get; set; } = null!;

    public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<ReviewRecruiter> ReviewRecruiters { get; set; } = new List<ReviewRecruiter>();
}

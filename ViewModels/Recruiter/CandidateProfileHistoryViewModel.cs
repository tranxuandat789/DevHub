namespace DevHub.ViewModels.Recruiter
{
    // Candidate-centric profile: full application history of one candidate across the recruiter's jobs.
    public class CandidateProfileHistoryViewModel
    {
        // --- Candidate info ---
        public int CandidateId { get; set; }
        public string FullName { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? PreferredLocation { get; set; }
        public string? SocialMediaUrl { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ExpectedSalaryMin { get; set; }
        public decimal? ExpectedSalaryMax { get; set; }
        public List<SkillItem> Skills { get; set; } = new();

        // --- Application history (newest first) ---
        public List<CandidateApplicationHistoryItem> ApplicationHistory { get; set; } = new();

        // --- Summary ---
        public int TotalApplications => ApplicationHistory.Count;
        public bool HasActiveApplication => ApplicationHistory
            .Any(a => a.ApplicationStatus is "PENDING" or "APPROVED" or "FINISHED");
    }

    public class SkillItem
    {
        public string TechName { get; set; } = "";
        public string? Level { get; set; }
    }

    public class CandidateApplicationHistoryItem
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = "";
        public string JobStatus { get; set; } = "";          // APPROVED / CLOSED / PENDING ...
        public string ApplicationStatus { get; set; } = "PENDING";
        public DateTime? AppliedAt { get; set; }
        public string? CvTitle { get; set; }
        public string? CoverLetterSnippet { get; set; }      // ~100 chars preview

        // Latest interview for this application (if any).
        public DateTime? LatestInterviewAt { get; set; }
        public string? InterviewStatus { get; set; }
        public string? InterviewLocation { get; set; }
        public string? MeetingLink { get; set; }

        public bool IsFrozen => (JobStatus ?? "").ToUpper() == "PENDING";
    }
}

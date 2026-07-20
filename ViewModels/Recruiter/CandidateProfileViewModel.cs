namespace DevHub.ViewModels.Recruiter
{
    // ViewModel for the candidate profile screen (UC-15) opened from the applicant list.
    public class CandidateProfileViewModel
    {
        public int ApplicationId { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = "";

        public string FullName { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? PreferredLocation { get; set; }
        public string? SocialMediaUrl { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ExpectedSalaryMin { get; set; }
        public decimal? ExpectedSalaryMax { get; set; }

        // Skills: (TechName, Level)
        public List<(string Tech, string? Level)> Skills { get; set; } = new();

        // CV attached to this application.
        public string? CvTitle { get; set; }
        public string? CvUrl { get; set; }
        public string? CoverLetter { get; set; }

        public string Status { get; set; } = "PENDING";
        public DateTime? AppliedAt { get; set; }
        public bool HasScheduledInterview { get; set; }

        // How many applications this candidate has at the recruiter's company (for the "view full history" link).
        public int TotalApplicationsAtCompany { get; set; }

        // Status of the parent job. When the recruiter has just edited an APPROVED job it returns to
        // PENDING for moderator re-review; while it is PENDING the application is "frozen" (read-only).
        public string JobStatus { get; set; } = "";
        public bool IsFrozen => (JobStatus ?? "").ToUpper() == "PENDING";

        // Approve/Reject are blocked while the job is frozen (pending re-review).
        public bool CanApprove => (Status ?? "").ToUpper() == "PENDING" && !IsFrozen;
        public bool CanReject => (Status ?? "").ToUpper() == "PENDING" && !IsFrozen;
        public bool CanHire => (Status ?? "").ToUpper() == "APPROVED" && !IsFrozen;
        public bool CanScheduleInterview => (Status ?? "").ToUpper() == "APPROVED" && ((JobStatus ?? "").ToUpper() == "APPROVED" || (JobStatus ?? "").ToUpper() == "ACTIVE");
    }
}

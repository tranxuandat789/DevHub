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

        public bool CanApprove => (Status ?? "").ToUpper() == "PENDING";
        public bool CanReject => (Status ?? "").ToUpper() == "PENDING";
        public bool CanScheduleInterview => (Status ?? "").ToUpper() == "APPROVED";
    }
}

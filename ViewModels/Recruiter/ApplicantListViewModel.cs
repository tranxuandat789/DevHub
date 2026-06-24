using DevHub.Models;

namespace DevHub.ViewModels.Recruiter
{
    // ViewModel for the applicant list for each-job and cross-job.
    public class ApplicantListViewModel
    {
        public bool IsCrossJob { get; set; }

        // Per-job context (null for cross-job).
        public int? JobId { get; set; }
        public string? JobTitle { get; set; }
        public string? JobStatus { get; set; }

        public List<ApplicantItem> Items { get; set; } = new();

        // Status counts for the tabs.
        public int CountAll { get; set; }
        public int CountPending { get; set; }
        public int CountApproved { get; set; }
        public int CountRejected { get; set; }

        // Pagination.
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        // Dropdown options (values bound from these lists only — typeahead just filters them).
        public List<CommonTechnology> TechOptions { get; set; } = new();
        public List<CommonJobPosition> PositionOptions { get; set; } = new();
        public List<string> LocationOptions { get; set; } = new();

        public ApplicantFilter Filter { get; set; } = new();
    }

    public class ApplicantItem
    {
        public int ApplicationId { get; set; }
        public int CandidateId { get; set; }
        public string FullName { get; set; } = "";
        public string? AvatarUrl { get; set; }       // Candidate.ImageUrl (fallback handled in view)
        public int? ExperienceYears { get; set; }
        public string? PreferredLocation { get; set; }
        public DateTime? AppliedAt { get; set; }
        public string Status { get; set; } = "PENDING";
        public List<string> TopSkills { get; set; } = new();
        public string? JobTitle { get; set; }        // shown in cross-job mode
    }
}

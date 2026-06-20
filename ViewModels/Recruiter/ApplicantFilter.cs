namespace DevHub.ViewModels.Recruiter
{
    // Filter/sort criteria for the recruiter applicant list (per-job & cross-job).
    public class ApplicantFilter
    {
        // Tech stack filter (ANY): match candidate having at least one of these techs.
        public List<int> TechIds { get; set; } = new();

        // Experience bucket key: "0-1" | "2-3" | "4-5" | "5+"
        public string? ExperienceBucket { get; set; }

        // Application status: null/"" = ALL | PENDING | APPROVED | REJECTED
        public string? Status { get; set; }

        // Cross-job only: filter applicants whose job has this position.
        public int? PositionId { get; set; }

        // Free-text keyword over candidate full name.
        public string? Keyword { get; set; }

        // Candidate preferred location (bound from a dropdown of present values).
        public string? Location { get; set; }

        // Sort: "applied_desc" (default) | "applied_asc" | "exp_desc" | "exp_asc"
        public string? Sort { get; set; }

        public int Page { get; set; } = 1;
    }

    // Experience buckets shared by the view (dropdown options) and the repository (range translation).
    public static class ExperienceBuckets
    {
        public record Bucket(string Key, string Label, int Min, int? Max);

        public static readonly IReadOnlyList<Bucket> All = new List<Bucket>
        {
            new("0-1", "0 - 1 năm", 0, 1),
            new("2-3", "2 - 3 năm", 2, 3),
            new("4-5", "4 - 5 năm", 4, 5),
            new("5+",  "Trên 5 năm", 5, null),
        };

        public static Bucket? Find(string? key)
            => string.IsNullOrWhiteSpace(key) ? null : All.FirstOrDefault(b => b.Key == key);
    }
}

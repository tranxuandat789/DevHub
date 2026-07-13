using DevHub.Models;

namespace DevHub.ViewModels.Recruiter
{
    // ViewModel for Managing job posts (/Recruiter/JobPost)
    public class JobPostManageViewModel
    {
        public List<JobPost> Items { get; set; } = new();

        // Search & filter criteria (optional)
        public string? Keyword { get; set; }
        public string? Status { get; set; }   

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }    // total of posts after filtering.
        public int TotalPages { get; set; }

        // Count all & count by each status 
        public int CountAll { get; set; }
        public int CountPending { get; set; }
        public int CountApproved { get; set; }
        public int CountRejected { get; set; }
        public int CountClosed { get; set; }

        // True if recruiter has any post.
        public bool HasAnyPost { get; set; }
    }
}

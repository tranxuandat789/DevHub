namespace DevHub.ViewModels.Recruiter
{
    public class RecruiterDashboardViewModel
    {
        public int TotalJobPosts { get; set; }
        public int TotalApplications { get; set; }
        public int TotalScheduledInterviews { get; set; }
        public int TotalCompletedInterviews { get; set; }
        public List<JobPostSummaryViewModel> JobPostApplicantCounts { get; set; } = new();
        public List<InterviewSummaryViewModel> ScheduledInterviews { get; set; } = new();
        public List<InterviewSummaryViewModel> CompletedInterviews { get; set; } = new();
    }

    public class JobPostSummaryViewModel
    {
        public string Title { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public int ApplicantCount { get; set; }
        public string? Status { get; set; }
    }

    public class InterviewSummaryViewModel
    {
        public int InterviewId { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public string? CandidateFullName { get; set; }
        public string? JobTitle { get; set; }
        public string? MeetingLink { get; set; }
        public string? Status { get; set; }
    }
}
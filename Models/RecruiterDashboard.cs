using System;
using System.Collections.Generic;

namespace DevHub.Models
{
    public class RecruiterDashboard
    {
        public int TotalJobPosts { get; set; }
        public int TotalApplications { get; set; }
        public int TotalScheduledInterviews { get; set; }
        public int TotalCompletedInterviews { get; set; }

        public JobStatsTableViewModel JobStats { get; set; } = new();
        public List<Interview> ScheduledInterviews { get; set; } = new List<Interview>();
        public List<Interview> CompletedInterviews { get; set; } = new List<Interview>();

        // Verification state.
        public bool IsVerified { get; set; }
        public bool HasPendingVerification { get; set; }

        // [#8] Conversion rate: interviews (scheduled + completed) / applications.
        public double InterviewConversionRate =>
            TotalApplications > 0
                ? Math.Round((double)(TotalScheduledInterviews + TotalCompletedInterviews) / TotalApplications * 100, 1)
                : 0;

        // [#3] Active service package.
        public bool HasActivePackage { get; set; }
        public string? ActivePackageName { get; set; }
        public int PostsRemaining { get; set; }
        public int PostsGranted { get; set; }
        public DateTime? PackageExpiry { get; set; }

        // [#4] Profile completion.
        public int ProfileCompletion { get; set; }
        public List<string> MissingProfileFields { get; set; } = new();

        // [#5] Jobs expiring soon.
        public List<ExpiringJobAlert> ExpiringJobs { get; set; } = new();

        // [#6] Recent applicants.
        public List<RecentApplicationItem> RecentApplications { get; set; } = new();

    }

    public class JobStatsTableViewModel
    {
        public List<JobStatsRow> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public string? FilterStatus { get; set; }
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
    }

    public class JobStatsRow
    {
        public int JobId { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime? CreatedAt { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int HiredCount { get; set; }
        public int TotalApplicationCount { get; set; }
    }

    // [#5]
    public class ExpiringJobAlert
    {
        public int JobId { get; set; }
        public string Title { get; set; } = "";
        public DateOnly Deadline { get; set; }
        public int DaysLeft { get; set; }
    }

    // [#6]
    public class RecentApplicationItem
    {
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public string JobTitle { get; set; } = "";
        public DateTime? AppliedAt { get; set; }
        public string ApplicationStatus { get; set; } = "";
    }
}

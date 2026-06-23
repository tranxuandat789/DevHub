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

        public List<JobPostApplicantCount> JobPostApplicantCounts { get; set; } = new List<JobPostApplicantCount>();

        public List<Interview> ScheduledInterviews { get; set; } = new List<Interview>();
        public List<Interview> CompletedInterviews { get; set; } = new List<Interview>();

        // Verification state.
        public bool IsVerified { get; set; }
        public bool HasPendingVerification { get; set; }

        // [#8] Conversion rate (scheduled interviews / applications).
        public double InterviewConversionRate =>
            TotalApplications > 0
                ? Math.Round((double)TotalScheduledInterviews / TotalApplications * 100, 1)
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

    public class JobPostApplicantCount
    {
        public int JobId { get; set; }
        public string? Title { get; set; }
        public int ApplicantCount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateOnly? Deadline { get; set; }
        // Resolved avatar URLs of recent applicants (for the avatar stack).
        public List<string> ApplicantAvatars { get; set; } = new();
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

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
    }

    public class JobPostApplicantCount
    {
        public int JobId { get; set; }
        public string? Title { get; set; }
        public int ApplicantCount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

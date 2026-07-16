using System;
using System.Collections.Generic;
using DevHub.Models;

namespace DevHub.ViewModels.Recruiter
{
    public class RecruiterInterviewViewModel
    {
        public List<JobPost> JobPosts { get; set; } = new List<JobPost>();
        public List<Interview> Interviews { get; set; } = new List<Interview>();
        
        public int? SelectedJobId { get; set; }
        public string ActiveTab { get; set; } = "scheduled"; // scheduled, completed_pending, passed, rejected
        public string SearchTerm { get; set; } = "";
        
        // Tab Counts
        public int ScheduledCount { get; set; }
        public int CompletedCount { get; set; }
        public int PassedCount { get; set; }
        public int RejectedCount { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}

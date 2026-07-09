
//5/2/2026 
//file view duyet bai dang
//author: Hoang Minh Kien



using System;
using System.Collections.Generic;
using DevHub.Models;

namespace DevHub.ViewModels.Moderator
{
    public class JobApprovalViewModel
    {
        public List<JobPost> JobPosts { get; set; } = new List<JobPost>();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortOrder { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }   // total pending posts (after filtering, before paging)
        public int TotalPages { get; set; }
        public int FromItem { get; set; }     // index of first item shown (1-based)
        public int ToItem { get; set; }       // index of last item shown
    }
}

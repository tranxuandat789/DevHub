
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
    }
}

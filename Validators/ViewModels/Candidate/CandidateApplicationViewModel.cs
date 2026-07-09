using System;
using System.Collections.Generic;

namespace DevHub.ViewModels.Candidate
{
    public class AppliedJobItemViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogoUrl { get; set; }
        public string? Location { get; set; }
        public string? WorkingModel { get; set; }
        public string? ExperienceLevel { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateOnly? Deadline { get; set; }
        public List<string> TechNames { get; set; } = new();
        public string? Status { get; set; }
        public DateTime? AppliedAt { get; set; }
    }

    public class AppliedJobPageViewModel
    {
        public List<AppliedJobItemViewModel> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchKeyword { get; set; }
        public string? FilterTimeRange { get; set; }
        public string? FilterStatus { get; set; }
    }
}

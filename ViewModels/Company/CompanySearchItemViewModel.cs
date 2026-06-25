using System.Collections.Generic;

namespace DevHub.ViewModels.Company
{
    public class CompanySearchItemViewModel
    {
        public int RecruiterId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyLogoUrl { get; set; }
        public string? CompanyAddress { get; set; }
        public string? Industry { get; set; }
        public double? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int JobCount { get; set; }
        
        // Ranking and Tech badges properties
        public int SystemRank { get; set; }
        public List<string> TechStacks { get; set; } = new();
    }
}

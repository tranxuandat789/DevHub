using System.Collections.Generic;

namespace DevHub.ViewModels.Company
{
    public class CompanySearchItemViewModel
    {
        public int CompanyId { get; set; }
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
        
        // Articles preview (up to 2 shown, total count for "read more" link)
        public List<(int ArticleId, string Title, string Slug)> ArticlesPreviews { get; set; } = new();
        public int TotalArticleCount { get; set; }
    }
}

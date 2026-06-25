namespace DevHub.Models
{
    public class DashboardViewModel
    {
        public int AppliedJobsCount { get; set; }
        public int SavedJobsCount { get; set; }
        public int InterviewsCount { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public decimal TotalRevenueMonth { get; set; }
        public int TotalPackagesMonth { get; set; }
        public decimal TotalRevenueQuarter { get; set; }
        public List<decimal> SixMonthRevenue { get; set; } = new List<decimal>();
        public List<RecentTransactionDto> RecentTransactions { get; set; } = new List<RecentTransactionDto>();

        // New Statistics
        public int TotalRecruiters { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalModerators { get; set; }
        public int ActiveJobPosts { get; set; }
        public int TotalAppliedCVs { get; set; }

        public List<TopRecruiterDto> TopRecruiters { get; set; } = new List<TopRecruiterDto>();
        
        public Dictionary<string, int> PackageDistribution { get; set; } = new Dictionary<string, int>();
    }

    public class RecentTransactionDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class TopRecruiterDto
    {
        public int RecruiterId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
    }
}

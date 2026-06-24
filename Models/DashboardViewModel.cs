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
    }

    public class RecentTransactionDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}

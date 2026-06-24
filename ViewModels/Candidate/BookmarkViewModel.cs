namespace DevHub.ViewModels.Candidate
{
    public class BookmarkViewModel
    {
        public int BookmarkId { get; set; }
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogoUrl { get; set; }
        public string? Location { get; set; }
        public string? WorkingModel { get; set; }
        public string? ExperienceLevel { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateTime? SavedAt { get; set; }
    }

    public class BookmarkPageViewModel
    {
        public List<BookmarkViewModel> Jobs { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Filter options cho dropdown
        public List<string> WorkingModelOptions { get; set; } = new();
        public List<string> ExperienceLevelOptions { get; set; } = new();
        public List<string> LocationOptions { get; set; } = new();

        // Giá trị filter hiện tại
        public string? FilterWorkingModel { get; set; }
        public string? FilterLevel { get; set; }
        public string? FilterLocation { get; set; }
    }
}

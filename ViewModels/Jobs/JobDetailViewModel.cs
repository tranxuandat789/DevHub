namespace DevHub.ViewModels.Jobs;

/// Complete data for the job details page.
/// Includes job information (left column) and summary company information (right column).

public class JobDetailViewModel
{
    public int JobId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    // Tỉnh/thành nơi làm việc (many-to-many). Thay cho trường location cũ.
    public List<string> Provinces { get; set; } = new();
    public string? WorkingModel { get; set; }
    public string? ExperienceLevel { get; set; }
    // RANGE | FROM | UPTO | NEGOTIABLE — quyết định cách hiển thị mức lương.
    public string SalaryType { get; set; } = "RANGE";
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public int? HiringQuota { get; set; }
    public DateOnly? Deadline { get; set; }
    public string? Description { get; set; }
    public string? Requirement { get; set; }
    public string? Benefit { get; set; }
    public List<string> TechNames { get; set; } = new();
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyLogoUrl { get; set; }
    public string? CompanyAddress { get; set; }
    public decimal? AverageRating { get; set; }
    public bool IsVerified { get; set; }
    public bool IsBookmarked { get; set; }
}

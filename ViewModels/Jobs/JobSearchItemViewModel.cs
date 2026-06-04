namespace DevHub.ViewModels.Jobs;

/// <summary>
/// Data displayed on each job card on the list page.
/// </summary>
public class JobSearchItemViewModel
{
    public int JobId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyLogoUrl { get; set; }
    public string? Location { get; set; }
    public string? WorkingModel { get; set; }
    public string? ExperienceLevel { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateOnly? Deadline { get; set; }
    public List<string> TechNames { get; set; } = new();
}

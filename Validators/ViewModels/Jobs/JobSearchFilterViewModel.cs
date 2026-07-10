namespace DevHub.ViewModels.Jobs;

/// <summary>
/// Query parameters for the job search page, bound from the URL query string.
/// </summary>
public class JobSearchFilterViewModel
{
    public string? Keyword { get; set; }
    public string? WorkingModel { get; set; }
    public string? ExperienceLevel { get; set; }
    public int? DesiredSalary { get; set; }
    // Quick-filter: lọc theo kỹ năng / thành phố / công ty
    public int? TechId { get; set; }
    public string? FilterLocation { get; set; }
    public int? RecruiterId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}

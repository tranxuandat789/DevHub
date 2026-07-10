namespace DevHub.ViewModels.Jobs;

/// <summary>
/// Aggregate model for the job search page (Index), including the job list, 
/// current filters, pagination, and filter options loaded from the DB.
/// </summary>
public class JobSearchPageViewModel
{
    public List<JobSearchItemViewModel> Jobs { get; set; } = new();
    public JobSearchFilterViewModel Filter { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<string> WorkingModelOptions { get; set; } = new();
    public List<string> ExperienceLevelOptions { get; set; } = new();
    // Data cho quick-filter panels (top 20 theo số job nhiều nhất)
    public List<(int TechId, string TechName, int JobCount)> TopTechs { get; set; } = new();
    public List<(string Location, int JobCount)> TopLocations { get; set; } = new();
    public List<(int RecruiterId, string CompanyName, string? LogoUrl, int JobCount)> TopCompanies { get; set; } = new();

    // Set các JobId mà ứng viên đã bookmark (chỉ có giá trị khi đã đăng nhập)
    public HashSet<int> BookmarkedJobIds { get; set; } = new();
}


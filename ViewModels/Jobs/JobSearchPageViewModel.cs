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
}

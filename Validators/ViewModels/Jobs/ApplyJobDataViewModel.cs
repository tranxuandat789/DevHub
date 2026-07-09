// Author: [your-name]
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Jobs;

public class ApplyJobDataViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    public int ProfileCompletion { get; set; }

    public int? DefaultCvId { get; set; }
    public string? DefaultCvTitle { get; set; }
}

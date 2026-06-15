// Author: [your-name]
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Jobs;

public class SubmitApplyViewModel
{
    [Required(ErrorMessage = "Mã công việc không được để trống")]
    public int JobId { get; set; }

    public string? CoverLetter { get; set; }
}

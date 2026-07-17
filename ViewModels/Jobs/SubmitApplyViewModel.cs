// Author: [your-name]
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Jobs;

public class SubmitApplyViewModel
{
    [Required(ErrorMessage = "Mã công việc không được để trống")]
    public int JobId { get; set; }

    public string? CoverLetter { get; set; }

    /// <summary>Nếu user upload CV mới trong modal, truyền CvId vào đây. Null = dùng CV mặc định.</summary>
    public int? CvId { get; set; }

    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

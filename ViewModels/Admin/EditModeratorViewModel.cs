// 04/06/2026-DatTX
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Admin;

// ViewModel for the Edit Moderator form (email is read-only, not editable)
public class EditModeratorViewModel
{
    public int    AdminId { get; set; }

    // Display only, not submitted
    public string Email   { get; set; } = "";

    [Required(ErrorMessage = "Username là bắt buộc.")]
    [MinLength(2, ErrorMessage = "Username phải có ít nhất 2 ký tự.")]
    [MaxLength(100)]
    [RegularExpression(@"^\s*[\p{L}\p{M}0-9_]+\s*$", ErrorMessage = "Username chỉ được chứa chữ cái, số và dấu gạch dưới.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [MinLength(2, ErrorMessage = "Họ và tên phải có ít nhất 2 ký tự.")]
    [MaxLength(255)]
    [RegularExpression(@"^\s*[\p{L}\p{M}]+(?: [\p{L}\p{M}]+)*\s*$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái và 1 dấu cách giữa các từ.")]
    public string FullName { get; set; } = "";

    /// <summary>Loại công việc hiện tại của moderator (nếu có)</summary>
    public string? CurrentTaskType { get; set; }

    /// <summary>Task type mới admin muốn gán (optional khi edit)</summary>
    public string? TaskType { get; set; }
}

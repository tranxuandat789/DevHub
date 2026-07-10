// 04/06/2026-DatTX
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Admin;

// ViewModel for the Create Moderator form
public class CreateModeratorViewModel
{
    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [MaxLength(255)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Username là bắt buộc.")]
    [MinLength(2, ErrorMessage = "Username phải có ít nhất 2 ký tự.")]
    [MaxLength(100)]
    // Username: only letters, digits, underscore — no spaces or special chars
    [RegularExpression(@"^\s*[\p{L}\p{M}0-9_]+\s*$", ErrorMessage = "Username chỉ được chứa chữ cái, số và dấu gạch dưới.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
    [MinLength(2, ErrorMessage = "Họ và tên phải có ít nhất 2 ký tự.")]
    [MaxLength(255)]
    // Full name: letters (including Vietnamese), spaces allowed but max 1 consecutive space, no special chars
    [RegularExpression(@"^\s*[\p{L}\p{M}]+(?: [\p{L}\p{M}]+)*\s*$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái và 1 dấu cách giữa các từ.")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Loại công việc là bắt buộc.")]
    public string TaskType { get; set; } = "";
}

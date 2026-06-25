using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email không đúng định dạng (vd: abc@gmail.com)")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}


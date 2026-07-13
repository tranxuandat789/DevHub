using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Auth
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).+$", ErrorMessage = "Mật khẩu không được chứa khoảng trắng, phải chứa ít nhất 1 chữ cái viết hoa và 1 ký tự đặc biệt.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Recruiter
{
    // Input model for the recruiter "change password" form.
    public class RecruiterChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        public string CurrentPassword { get; set; } = null!;

        // New password must be at least 8 chars and contain at least 1 uppercase, 1 digit, 1 special char.
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm tối thiểu 1 chữ hoa, 1 chữ số và 1 ký tự đặc biệt.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Auth
{
    public class RegisterRecruiterViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và gồm đúng 10 chữ số!")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự!")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu!")]
        public string VerifyPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // FullName: ít nhất 2 từ, không chứa ký tự đặc biệt
            var words = FullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                yield return new ValidationResult(
                    "Họ và tên phải có ít nhất 2 từ!",
                    new[] { nameof(FullName) });

            if (!System.Text.RegularExpressions.Regex.IsMatch(FullName, @"^[\p{L}\s]+$"))
                yield return new ValidationResult(
                    "Họ và tên không được chứa ký tự đặc biệt hoặc chữ số!",
                    new[] { nameof(FullName) });

            // Mật khẩu khớp
            if (Password != VerifyPassword)
                yield return new ValidationResult(
                    "Mật khẩu và mật khẩu nhập lại không khớp!",
                    new[] { nameof(VerifyPassword) });
        }
    }
}
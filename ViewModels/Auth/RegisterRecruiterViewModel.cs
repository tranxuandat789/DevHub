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

        [Required(ErrorMessage = "Vị trí không được để trống!")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên công ty không được để trống!")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ công ty không được để trống!")]
        public string CompanyAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngành nghề không được để trống!")]
        public string Industry { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // FullName: ít nhất 2 từ, không chứa ký tự đặc biệt
            var words = FullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                yield return new ValidationResult(
                    "Họ và tên phải có ít nhất 2 kí tự!",
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

            if (!string.IsNullOrWhiteSpace(Position))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(Position, @"\d"))
                    yield return new ValidationResult("phần position không bao gồm số", new[] { nameof(Position) });
                else if (!System.Text.RegularExpressions.Regex.IsMatch(Position, @"^[\p{L}\s]+$"))
                    yield return new ValidationResult("Vị trí không bao gồm ký tự đặc biệt", new[] { nameof(Position) });
            }

            if (!string.IsNullOrWhiteSpace(Industry))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(Industry, @"\d"))
                    yield return new ValidationResult("phần industry không bao gồm số", new[] { nameof(Industry) });
                else if (!System.Text.RegularExpressions.Regex.IsMatch(Industry, @"^[\p{L}\s_\-,.]+$"))
                    yield return new ValidationResult("Ngành nghề không bao gồm ký tự đặc biệt", new[] { nameof(Industry) });
            }

            if (!string.IsNullOrWhiteSpace(CompanyName) && !System.Text.RegularExpressions.Regex.IsMatch(CompanyName, @"^[\p{L}\d\s_\-]+$"))
            {
                yield return new ValidationResult("Tên công ty không bao gồm ký tự đặc biệt", new[] { nameof(CompanyName) });
            }

            if (!string.IsNullOrWhiteSpace(CompanyAddress) && !System.Text.RegularExpressions.Regex.IsMatch(CompanyAddress, @"^[\p{L}\d\s,.\-_]+$"))
            {
                yield return new ValidationResult("Địa chỉ công ty không bao gồm ký tự đặc biệt", new[] { nameof(CompanyAddress) });
            }
        }
    }
}
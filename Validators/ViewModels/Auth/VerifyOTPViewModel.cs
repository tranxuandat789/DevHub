using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Auth
{
    public class VerifyOTPViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã OTP.")]
        public string OTP { get; set; } = string.Empty;
    }
}

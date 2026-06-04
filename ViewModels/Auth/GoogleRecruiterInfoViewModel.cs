using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Auth;

public class GoogleRecruiterInfoViewModel
{
    [Required]
    public string Email { get; set; } = "";      // readonly — lấy từ session

    public string FullName { get; set; } = "";   // readonly — lấy từ Google

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "SĐT Việt Nam phải bắt đầu bằng 0 và đủ 10 chữ số")]
    public string Phone { get; set; } = "";
}


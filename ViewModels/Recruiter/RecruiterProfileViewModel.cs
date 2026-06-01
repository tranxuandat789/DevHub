using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Recruiter
{
    public class RecruiterProfileViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } = null!;

        public string? Position { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$", ErrorMessage = "Số điện thoại không đúng định dạng di động Việt Nam.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Tên công ty không được để trống.")]
        public string CompanyName { get; set; } = null!;

        public string? CompanyAddress { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public string? CompanyDescription { get; set; }

        [Url(ErrorMessage = "Đường dẫn Website không hợp lệ (Ví dụ hợp lệ: https://company.com).")]
        public string? Website { get; set; }

        public string? Industry { get; set; }

        [Required(ErrorMessage = "Mã số thuế không được để trống.")]
        [RegularExpression(@"^[0-9]{10}(-[0-9]{3})?$", ErrorMessage = "Mã số thuế phải bao gồm 10 chữ số (hoặc 13 chữ số kèm dấu gạch ngang cho chi nhánh).")]
        public string? TaxCode { get; set; }

        public string? BusinessLicenseUrl { get; set; }
        public string? AdditionalDocumentsUrl { get; set; }
        public bool? IsVerified { get; set; }

        // profile completeness percentage to determine when recruiter can access features
        public int ProfileCompleteness { get; set; }
    }
}

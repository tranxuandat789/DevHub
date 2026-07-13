//03/06/2026 DatTX
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Candidate
{
    public class UpdateBasicInfoViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự!")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ và tên không được chứa ký tự đặc biệt hoặc số!")]
        public string FullName { get; set; } = null!;
        [RegularExpression(@"^(0(3[2-9]|5[5689]|7[06-9]|8[1-9]|9[0-9]))[0-9]{7}$", ErrorMessage = "Số điện thoại không hợp lệ hoặc không đúng đầu số Việt Nam!")]
        public string? Phone { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Gender { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [Url(ErrorMessage = "Link mạng xã hội phải bắt đầu bằng http:// hoặc https://")]
        public string? SocialMediaUrl { get; set; }
        
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "Mức lương tối thiểu không hợp lệ (tối đa 10 chữ số)")]
        public decimal? ExpectedSalaryMin { get; set; }
        
        [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "Mức lương tối đa không hợp lệ (tối đa 10 chữ số)")]
        public decimal? ExpectedSalaryMax { get; set; }
        
        [StringLength(100)]
        [RegularExpression(@"^[\p{L}0-9\s]+$", ErrorMessage = "Địa điểm mong muốn không được chứa ký tự đặc biệt!")]
        public string? PreferredLocation { get; set; }
        
        [StringLength(50)]
        public string? PreferredWorkingModel { get; set; }
        
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm không hợp lệ (từ 0 đến 50)")]
        public int? ExperienceYears { get; set; }
        
        public bool CvSearchable { get; set; }
    }
}

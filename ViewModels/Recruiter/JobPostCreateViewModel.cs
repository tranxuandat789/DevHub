//AnhPT-02/06/2026
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Recruiter
{
    public class JobPostCreateViewModel : IValidatableObject
    {
        // Free-text info fields: allows all typical text characters, blocks basic HTML tags (<>)
        private const string TextPattern = @"^[^<>]*$";

        [Required(ErrorMessage = "Tiêu đề bài đăng không được để trống.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
        [RegularExpression(TextPattern, ErrorMessage = "Tiêu đề chứa ký tự không hợp lệ.")]
        public string Title { get; set; } = null!;

        // Chỉ cho phép chọn duy nhất 1 Vị trí công việc (Bắt buộc chọn)
        [Required(ErrorMessage = "Vui lòng chọn một vị trí công việc từ danh sách.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vị trí công việc được chọn không hợp lệ.")]
        public int PositionId { get; set; }

        // Cho phép chọn nhiều Công nghệ (Danh sách ID)
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một công nghệ liên quan.")]
        public List<int> TechnologyIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Mô tả công việc chứa ký tự không hợp lệ.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Yêu cầu công việc không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Yêu cầu công việc chứa ký tự không hợp lệ.")]
        public string Requirement { get; set; } = null!;

        [Required(ErrorMessage = "Quyền lợi ứng viên không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Quyền lợi ứng viên chứa ký tự không hợp lệ.")]
        public string Benefit { get; set; } = null!;

        [Required(ErrorMessage = "Yêu cầu cấp bậc/Kinh nghiệm không được để trống.")]
        public string ExperienceLevel { get; set; } = null!;

        // Tỉnh/thành nơi làm việc (chọn 1 hoặc nhiều từ bảng province).
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một tỉnh/thành làm việc.")]
        [MinLength(1, ErrorMessage = "Vui lòng chọn ít nhất một tỉnh/thành làm việc.")]
        public List<int> ProvinceIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Hình thức làm việc không được để trống.")]
        public string WorkingModel { get; set; } = null!;

        // Loại lương: RANGE (khoảng cụ thể) | UPTO (lên đến) | NEGOTIABLE (thỏa thuận) | FROM (từ).
        [Required(ErrorMessage = "Vui lòng chọn loại lương.")]
        public string SalaryType { get; set; } = "RANGE";

        // Nullable: NEGOTIABLE không cần lương, UPTO chỉ cần tối đa. Kiểm tra theo SalaryType ở Validate().
        [Range(0, double.MaxValue, ErrorMessage = "Mức lương tối thiểu phải là số dương.")]
        public decimal? SalaryMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Mức lương tối đa phải là số dương.")]
        public decimal? SalaryMax { get; set; }

        [Required(ErrorMessage = "Số lượng tuyển dụng không được để trống.")]
        [Range(1, 1000, ErrorMessage = "Số lượng tuyển dụng phải nằm trong khoảng từ 1 đến 1000 người.")]
        public int HiringQuota { get; set; }

        [Required(ErrorMessage = "Hạn chót nộp hồ sơ không được để trống.")]
        public DateOnly Deadline { get; set; }

        [RegularExpression(TextPattern, ErrorMessage = "Kỹ năng chứa ký tự không hợp lệ.")]
        public string? Skill { get; set; }

        // Kiểm tra lương phụ thuộc loại lương đã chọn.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var type = (SalaryType ?? "").ToUpperInvariant();
            switch (type)
            {
                case "RANGE":
                    if (SalaryMin is null)
                        yield return new ValidationResult("Vui lòng nhập mức lương tối thiểu.", new[] { nameof(SalaryMin) });
                    if (SalaryMax is null)
                        yield return new ValidationResult("Vui lòng nhập mức lương tối đa.", new[] { nameof(SalaryMax) });
                    if (SalaryMin is not null && SalaryMax is not null && SalaryMax < SalaryMin)
                        yield return new ValidationResult("Mức lương tối đa phải lớn hơn hoặc bằng mức lương tối thiểu.", new[] { nameof(SalaryMax) });
                    break;
                case "FROM":
                    if (SalaryMin is null)
                        yield return new ValidationResult("Vui lòng nhập mức lương tối thiểu.", new[] { nameof(SalaryMin) });
                    break;
                case "UPTO":
                    if (SalaryMax is null)
                        yield return new ValidationResult("Vui lòng nhập mức lương tối đa.", new[] { nameof(SalaryMax) });
                    break;
                case "NEGOTIABLE":
                    break;
                default:
                    yield return new ValidationResult("Loại lương không hợp lệ.", new[] { nameof(SalaryType) });
                    break;
            }
        }
    }
}

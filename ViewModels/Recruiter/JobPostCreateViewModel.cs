//AnhPT-02/06/2026
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Recruiter
{
    public class JobPostCreateViewModel
    {
        // Only letters (incl. Vietnamese), digits and blanks are allowed for free-text info fields.
        private const string TextPattern = @"^[\p{L}0-9\s]+$";

        [Required(ErrorMessage = "Tiêu đề bài đăng không được để trống.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
        [RegularExpression(TextPattern, ErrorMessage = "Tiêu đề chỉ được chứa chữ, số và khoảng trắng.")]
        public string Title { get; set; } = null!;

        // Chỉ cho phép chọn duy nhất 1 Vị trí công việc (Bắt buộc chọn)
        [Required(ErrorMessage = "Vui lòng chọn một vị trí công việc từ danh sách.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vị trí công việc được chọn không hợp lệ.")]
        public int PositionId { get; set; }

        // Cho phép chọn nhiều Công nghệ (Danh sách ID)
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một công nghệ liên quan.")]
        public List<int> TechnologyIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Mô tả công việc không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Mô tả công việc chỉ được chứa chữ, số và khoảng trắng.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Yêu cầu công việc không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Yêu cầu công việc chỉ được chứa chữ, số và khoảng trắng.")]
        public string Requirement { get; set; } = null!;

        [Required(ErrorMessage = "Quyền lợi ứng viên không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Quyền lợi ứng viên chỉ được chứa chữ, số và khoảng trắng.")]
        public string Benefit { get; set; } = null!;

        [Required(ErrorMessage = "Yêu cầu cấp bậc/Kinh nghiệm không được để trống.")]
        public string ExperienceLevel { get; set; } = null!;

        [Required(ErrorMessage = "Địa điểm làm việc không được để trống.")]
        [RegularExpression(TextPattern, ErrorMessage = "Địa điểm chỉ được chứa chữ, số và khoảng trắng.")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "Hình thức làm việc không được để trống.")]
        public string WorkingModel { get; set; } = null!;

        [Required(ErrorMessage = "Mức lương tối thiểu không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Mức lương tối thiểu phải là số dương.")]
        public decimal SalaryMin { get; set; }

        [Required(ErrorMessage = "Mức lương tối đa không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Mức lương tối đa phải là số dương.")]
        public decimal SalaryMax { get; set; }

        [Required(ErrorMessage = "Số lượng tuyển dụng không được để trống.")]
        [Range(1, 1000, ErrorMessage = "Số lượng tuyển dụng phải nằm trong khoảng từ 1 đến 1000 người.")]
        public int HiringQuota { get; set; }

        [Required(ErrorMessage = "Hạn chót nộp hồ sơ không được để trống.")]
        public DateOnly Deadline { get; set; }

        [RegularExpression(TextPattern, ErrorMessage = "Kỹ năng chỉ được chứa chữ, số và khoảng trắng.")]
        public string? Skill { get; set; }
    }
}

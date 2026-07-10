using System.ComponentModel.DataAnnotations;
using DevHub.Models;

namespace DevHub.ViewModels.Candidate;

public class WriteReviewViewModel
{
    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public bool AlreadyReviewed { get; set; }
    public ReviewCompany? ExistingReview { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn đánh giá tổng quan")]
    [Range(1, 5, ErrorMessage = "Đánh giá từ 1 đến 5 sao")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn mức độ hài lòng về lương & thưởng")]
    public int? SalaryRating { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn mức độ hài lòng về đào tạo & học hỏi")]
    public int? TrainingRating { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn mức độ hài lòng về sự quan tâm nhân viên")]
    public int? CareRating { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn mức độ hài lòng về văn hóa công ty")]
    public int? CultureRating { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn mức độ hài lòng về không gian làm việc")]
    public int? WorkspaceRating { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập ưu điểm")]
    [MinLength(10, ErrorMessage = "Ưu điểm cần dài ít nhất 10 ký tự")]
    public string Pros { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập nhược điểm")]
    [MinLength(10, ErrorMessage = "Nhược điểm cần dài ít nhất 10 ký tự")]
    public string Cons { get; set; } = null!;

    public string? OtPolicy { get; set; }
    
    public bool? Recommend { get; set; }
}

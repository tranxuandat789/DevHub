using System.ComponentModel.DataAnnotations;

namespace DevHub.Validators.ViewModels.Recruiter
{
    public class ArticleCreateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = null!;

        public string? ThumbnailUrl { get; set; }
    }

    public class ArticleEditViewModel
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = null!;

        public string? ThumbnailUrl { get; set; }
    }
}

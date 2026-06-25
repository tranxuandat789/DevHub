using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Recruiter
{
    public class BlogPostCreateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc.")]
        public string Content { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public string? Author { get; set; }

        public string? Tags { get; set; }

        public string? actionType { get; set; }
    }
}

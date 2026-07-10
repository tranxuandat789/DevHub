using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DevHub.ViewModels.Moderator
{
    public class CreateBlogViewModel
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = null!;
        
        public string? Tag { get; set; } = "Chung";

        [StringLength(255, ErrorMessage = "Tên tác giả không được vượt quá 255 ký tự")]
        public string? AuthorName { get; set; }

        public IFormFile? ThumbnailImage { get; set; }

        public string? ThumbnailUrl { get; set; }

        public bool IsPublished { get; set; } = false;
    }
}

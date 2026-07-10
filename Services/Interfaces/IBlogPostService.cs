using DevHub.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DevHub.Services.Interfaces
{
    public interface IBlogPostService
    {
        Task<(IEnumerable<BlogPost> Blogs, int TotalPages, int TotalItems)> GetFilteredBlogsAsync(string keyword, string dateFrom, string status, string sortBy, int page, int pageSize, bool forModerator = false);
        Task<BlogPost?> GetPostByIdAsync(int id);
        Task<bool> ToggleVisibilityAsync(int id);
        Task<bool> DeletePostAsync(int id);
        Task<BlogPost> CreateBlogPostAsync(int publisherId, string title, string content, string? thumbnailUrl, string? tag, bool isPublished, string? authorName);
        Task<bool> UpdateBlogPostAsync(int id, string title, string content, string? thumbnailUrl, string? tag, bool isPublished, string? authorName);
    }
}

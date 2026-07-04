using DevHub.Models;
using DevHub.ViewModels.Recruiter;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DevHub.Services.Interfaces
{
    public interface IBlogPostService
    {
        Task<(IEnumerable<BlogPost> Blogs, int TotalPages, int TotalItems)> GetFilteredBlogsAsync(string keyword, string dateFrom, string status, string sortBy, int page, int pageSize, bool forModerator = false);
        Task CreatePostAsync(BlogPostCreateViewModel model, int? publisherId);
        Task<BlogPost?> GetPostByIdAsync(int id);
        Task<bool> EditPostAsync(int id, BlogPostEditViewModel model);
        Task<bool> ToggleVisibilityAsync(int id);
        Task<bool> DeletePostAsync(int id);
    }
}

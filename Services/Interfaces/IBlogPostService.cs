using DevHub.Models;
using DevHub.ViewModels.Recruiter;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DevHub.Services.Interfaces
{
    public interface IBlogPostService
    {
        Task<(IEnumerable<BlogPost> Blogs, int TotalPages, int TotalItems)> GetFilteredBlogsAsync(string keyword, string dateFrom, string status, string sortBy, int page, int pageSize, int? authorId = null, bool forModerator = false);
        Task CreatePostAsync(BlogPostCreateViewModel model, int? publisherId, int? authorId);
        Task<BlogPost?> GetPostByIdAsync(int id);
        Task<bool> EditPostAsync(int id, BlogPostEditViewModel model);
        Task<bool> ToggleVisibilityAsync(int id);
        Task<bool> DeletePostAsync(int id);
        Task<bool> ApprovePostAsync(int id, int approverId);
        Task<bool> RejectPostAsync(int id, int approverId, string reason);
    }
}

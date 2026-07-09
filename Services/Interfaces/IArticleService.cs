using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IArticleService
{
    Task<Article?> GetArticleByIdAsync(int id);
    Task<Article> CreateArticleAsync(int recruiterId, string title, string content, string thumbnailUrl);
    Task UpdateArticleAsync(int recruiterId, int articleId, string title, string content, string thumbnailUrl);
    Task<List<Article>> GetArticlesForRecruiterAsync(int recruiterId);
    Task DeleteArticleAsync(int recruiterId, int articleId);
    Task ToggleArticleVisibilityAsync(int recruiterId, int articleId);
    Task SubmitArticleForReviewAsync(int recruiterId, int articleId);

    // Moderator methods
    Task<(IEnumerable<Article> Articles, int TotalPages, int TotalItems)> GetArticlesForModeratorAsync(string keyword, string dateFrom, string status, string companyName, int page, int pageSize);
    Task HideArticleByModAsync(int articleId, string reason);
    Task DeleteArticleByModAsync(int articleId, string reason);
}

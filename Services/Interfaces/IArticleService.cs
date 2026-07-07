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
    Task SubmitArticleForReviewAsync(int recruiterId, int articleId);
    Task<(List<Article> Articles, int TotalPages, int TotalItems)> GetArticlesForModeratorAsync(int? companyId, string keyword, int page, int pageSize);
    Task<bool> DeleteArticleByModeratorAsync(int articleId, int moderatorId, string reason);
}

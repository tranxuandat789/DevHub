using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(int id);
    Task<Article> CreateAsync(Article article);
    Task UpdateAsync(Article article);
    Task<List<Article>> GetArticlesByCompanyOrRecruiterAsync(int? companyId, int recruiterId);
    Task<List<Article>> GetArticlesByCompanyAsync(int companyId);
    Task<List<Article>> GetPendingArticlesByModeratorAsync(int moderatorId);
    Task<(IEnumerable<Article> Articles, int TotalPages, int TotalItems)> GetArticlesForModeratorAsync(string keyword, string dateFrom, string status, string companyName, int page, int pageSize);
    Task DeleteAsync(int id);
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(int id);
    Task<Article> CreateAsync(Article article);
    Task UpdateAsync(Article article);
    Task<List<Article>> GetArticlesByCompanyAsync(int companyId);
    Task<List<Article>> GetPendingArticlesByModeratorAsync(int moderatorId);
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class ArticleRepository : IArticleRepository
{
    private readonly ItrecruitmentDbContext _context;

    public ArticleRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        return await _context.Articles
            .Include(a => a.Company)
            .FirstOrDefaultAsync(a => a.ArticleId == id);
    }

    public async Task<Article> CreateAsync(Article article)
    {
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task UpdateAsync(Article article)
    {
        _context.Articles.Update(article);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Article>> GetArticlesByCompanyAsync(int companyId)
    {
        return await _context.Articles
            .Where(a => a.CompanyId == companyId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Article>> GetPendingArticlesByModeratorAsync(int moderatorId)
    {
        return await _context.Articles
            .Include(a => a.Company)
            .Where(a => a.ApproverId == moderatorId && a.Status == "PENDING")
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}

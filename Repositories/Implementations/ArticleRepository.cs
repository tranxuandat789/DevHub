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

    public async Task<(List<Article> Articles, int TotalCount)> GetArticlesForModerationAsync(int? companyId, string keyword, int page, int pageSize)
    {
        var query = _context.Articles.Include(a => a.Company).AsQueryable();

        if (companyId.HasValue)
        {
            query = query.Where(a => a.CompanyId == companyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(a => a.Title.Contains(keyword) || a.Company.CompanyName.Contains(keyword));
        }

        int totalCount = await query.CountAsync();
        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (articles, totalCount);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article == null) return false;

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }
}

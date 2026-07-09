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

    public async Task<List<Article>> GetArticlesByCompanyOrRecruiterAsync(int? companyId, int recruiterId)
    {
        // Bài báo gắn với Company, không có cột recruiter_id trong DB
        // Nếu có companyId thì lấy theo company, không thì trả về rỗng
        if (!companyId.HasValue) return new List<Article>();
        
        return await _context.Articles
            .Where(a => a.CompanyId == companyId.Value)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
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

    public async Task DeleteAsync(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article != null)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(IEnumerable<Article> Articles, int TotalPages, int TotalItems)> GetArticlesForModeratorAsync(string keyword, string dateFrom, string status, string companyName, int page, int pageSize)
    {
        var query = _context.Articles
            .Include(a => a.Company)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(a => a.Title.Contains(keyword) || a.Content.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(dateFrom) && System.DateTime.TryParse(dateFrom, out var parsedDate))
        {
            query = query.Where(a => a.CreatedAt >= parsedDate.Date);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(companyName))
        {
            query = query.Where(a => a.Company.CompanyName.Contains(companyName));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (articles, totalPages, totalItems);
    }
}

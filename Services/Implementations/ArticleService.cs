using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepo;
    private readonly IRecruiterRepository _recruiterRepo;
    private readonly ICompanyPackageHistoryRepository _packageRepo;
    private readonly IModAssignmentService _modAssignmentService;

    public ArticleService(
        IArticleRepository articleRepo,
        IRecruiterRepository recruiterRepo,
        ICompanyPackageHistoryRepository packageRepo,
        IModAssignmentService modAssignmentService)
    {
        _articleRepo = articleRepo;
        _recruiterRepo = recruiterRepo;
        _packageRepo = packageRepo;
        _modAssignmentService = modAssignmentService;
    }

    private string GenerateSlug(string title)
    {
        string slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim();
        return slug + "-" + Guid.NewGuid().ToString().Substring(0, 8);
    }

    public async Task<Article?> GetArticleByIdAsync(int id)
    {
        return await _articleRepo.GetByIdAsync(id);
    }

    public async Task<Article> CreateArticleAsync(int recruiterId, string title, string content, string thumbnailUrl)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        if (recruiter?.CompanyId == null)
            throw new InvalidOperationException("Recruiter does not belong to a company.");

        var article = new Article
        {
            CompanyId = recruiter.CompanyId.Value,
            Title = title,
            Slug = GenerateSlug(title),
            Content = content,
            ThumbnailUrl = thumbnailUrl,
            Status = "PENDING",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // Assign moderator based on company package
        var package = await _packageRepo.GetActivePackageForCompanyAsync(recruiter.CompanyId.Value);
        if (package != null)
        {
            article.ApproverId = await _modAssignmentService.GetAssignedModeratorAsync(package.ServiceId);
        }

        return await _articleRepo.CreateAsync(article);
    }

    public async Task UpdateArticleAsync(int recruiterId, int articleId, string title, string content, string thumbnailUrl)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        if (recruiter?.CompanyId == null)
            throw new InvalidOperationException("Recruiter does not belong to a company.");

        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null || article.CompanyId != recruiter.CompanyId)
            throw new InvalidOperationException("Article not found or unauthorized.");

        if (article.Status == "APPROVED")
            throw new InvalidOperationException("Cannot edit an approved article.");

        article.Title = title;
        article.Slug = GenerateSlug(title);
        article.Content = content;
        article.ThumbnailUrl = thumbnailUrl;
        article.UpdatedAt = DateTime.Now;

        await _articleRepo.UpdateAsync(article);
    }

    public async Task<List<Article>> GetArticlesForRecruiterAsync(int recruiterId)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        if (recruiter?.CompanyId == null)
            return new List<Article>();

        return await _articleRepo.GetArticlesByCompanyAsync(recruiter.CompanyId.Value);
    }

    public async Task SubmitArticleForReviewAsync(int recruiterId, int articleId)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null || article.CompanyId != recruiter?.CompanyId)
            throw new InvalidOperationException("Article not found or unauthorized.");

        article.Status = "PENDING";
        
        // Re-assign moderator just in case
        var package = await _packageRepo.GetActivePackageForCompanyAsync(recruiter.CompanyId.Value);
        if (package != null)
        {
            article.ApproverId = await _modAssignmentService.GetAssignedModeratorAsync(package.ServiceId);
        }

        await _articleRepo.UpdateAsync(article);
    }
    public async Task<(List<Article> Articles, int TotalPages, int TotalItems)> GetArticlesForModeratorAsync(int? companyId, string keyword, int page, int pageSize)
    {
        var (articles, totalCount) = await _articleRepo.GetArticlesForModerationAsync(companyId, keyword, page, pageSize);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return (articles, totalPages, totalCount);
    }

    public async Task<bool> DeleteArticleByModeratorAsync(int articleId, int moderatorId, string reason)
    {
        // Deletion logic, audit log can be handled here or in controller.
        // We will just delete it here. The Controller will add AuditLog.
        return await _articleRepo.DeleteAsync(articleId);
    }
}

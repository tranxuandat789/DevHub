using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.Helpers;

namespace DevHub.Services.Implementations;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepo;
    private readonly IRecruiterRepository _recruiterRepo;
    private readonly ICompanyPackageHistoryRepository _packageRepo;
    private readonly IModAssignmentService _modAssignmentService;
    private readonly EmailHelper _emailHelper;

    public ArticleService(
        IArticleRepository articleRepo,
        IRecruiterRepository recruiterRepo,
        ICompanyPackageHistoryRepository packageRepo,
        IModAssignmentService modAssignmentService,
        EmailHelper emailHelper)
    {
        _articleRepo = articleRepo;
        _recruiterRepo = recruiterRepo;
        _packageRepo = packageRepo;
        _modAssignmentService = modAssignmentService;
        _emailHelper = emailHelper;
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
        
        if (recruiter == null || recruiter.CompanyId == null)
        {
            throw new InvalidOperationException("Tài khoản của bạn chưa được liên kết với công ty nào. Vui lòng cập nhật hồ sơ công ty trước khi tạo bài viết.");
        }

        var article = new Article
        {
            CompanyId = recruiter.CompanyId,
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
        
        if (recruiter == null || recruiter.CompanyId == null)
        {
            throw new InvalidOperationException("Tài khoản của bạn chưa được liên kết với công ty nào.");
        }

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

        return await _articleRepo.GetArticlesByCompanyOrRecruiterAsync(recruiter?.CompanyId, recruiterId);
    }

    public async Task SubmitArticleForReviewAsync(int recruiterId, int articleId)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        if (recruiter == null || recruiter.CompanyId == null)
            throw new InvalidOperationException("Tài khoản của bạn chưa được liên kết với công ty nào.");

        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null || article.CompanyId != recruiter.CompanyId)
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

    public async Task DeleteArticleAsync(int recruiterId, int articleId)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        if (recruiter == null || recruiter.CompanyId == null)
            throw new InvalidOperationException("Tài khoản của bạn chưa được liên kết với công ty nào.");

        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null || article.CompanyId != recruiter.CompanyId)
            throw new InvalidOperationException("Article not found or unauthorized.");

        await _articleRepo.DeleteAsync(articleId);
    }

    public async Task ToggleArticleVisibilityAsync(int recruiterId, int articleId)
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        if (recruiter == null || recruiter.CompanyId == null)
            throw new InvalidOperationException("Tài khoản của bạn chưa được liên kết với công ty nào.");

        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null || article.CompanyId != recruiter.CompanyId)
            throw new InvalidOperationException("Article not found or unauthorized.");

        if (article.Status == "HIDDEN_BY_MOD")
            throw new InvalidOperationException("This article was hidden by a moderator and cannot be toggled until updated.");

        if (article.Status == "PUBLISHED")
            article.Status = "HIDDEN";
        else if (article.Status == "HIDDEN")
            article.Status = "PUBLISHED";
        
        await _articleRepo.UpdateAsync(article);
    }

    public async Task<(IEnumerable<Article> Articles, int TotalPages, int TotalItems)> GetArticlesForModeratorAsync(string keyword, string dateFrom, string status, string companyName, int page, int pageSize)
    {
        return await _articleRepo.GetArticlesForModeratorAsync(keyword, dateFrom, status, companyName, page, pageSize);
    }

    public async Task HideArticleByModAsync(int articleId, string reason)
    {
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null)
            throw new InvalidOperationException("Article not found.");

        article.Status = "HIDDEN_BY_MOD";
        article.RejectReason = reason;
        await _articleRepo.UpdateAsync(article);

        var recruiters = await _recruiterRepo.GetRecruitersByCompanyIdAsync(article.CompanyId.Value);
        foreach (var recruiter in recruiters)
        {
            if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email))
            {
                var subject = "Thông báo: Bài viết của bạn đã bị ẩn";
                var content = $@"<p>Chào {recruiter.FullName},</p>
                               <p>Bài viết ""<strong>{article.Title}</strong>"" của công ty bạn đã bị Ban quản trị ẩn với lý do:</p>
                               <blockquote style='border-left: 4px solid #ccc; padding-left: 10px;'>{reason}</blockquote>
                               <p>Vui lòng cập nhật lại nội dung bài viết nếu bạn muốn bài viết được hiển thị trở lại.</p>";
                var body = EmailHelper.GetBaseTemplate("Thông báo ẩn bài viết", content);
                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, subject, body);
            }
        }
    }

    public async Task DeleteArticleByModAsync(int articleId, string reason)
    {
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null)
            throw new InvalidOperationException("Article not found.");

        var title = article.Title;
        var companyId = article.CompanyId;

        await _articleRepo.DeleteAsync(articleId);

        var recruiters = await _recruiterRepo.GetRecruitersByCompanyIdAsync(companyId.Value);
        foreach (var recruiter in recruiters)
        {
            if (recruiter.RecruiterNavigation != null && !string.IsNullOrEmpty(recruiter.RecruiterNavigation.Email))
            {
                var subject = "Thông báo: Bài viết của bạn đã bị xóa";
                var content = $@"<p>Chào {recruiter.FullName},</p>
                               <p>Bài viết ""<strong>{title}</strong>"" của công ty bạn đã bị Ban quản trị xóa với lý do:</p>
                               <blockquote style='border-left: 4px solid #ccc; padding-left: 10px;'>{reason}</blockquote>
                               <p>Nếu bạn có thắc mắc, vui lòng liên hệ với bộ phận hỗ trợ.</p>";
                var body = EmailHelper.GetBaseTemplate("Thông báo xóa bài viết", content);
                await _emailHelper.SendEmailAsync(recruiter.RecruiterNavigation.Email, subject, body);
            }
        }
    }
}

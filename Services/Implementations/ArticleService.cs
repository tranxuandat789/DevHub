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
    private readonly EmailHelper _emailHelper;
    private readonly IAdminRepository _adminRepo;
    private readonly INotificationService _notificationService;

    public ArticleService(
        IArticleRepository articleRepo,
        IRecruiterRepository recruiterRepo,
        EmailHelper emailHelper,
        IAdminRepository adminRepo,
        INotificationService notificationService)
    {
        _articleRepo = articleRepo;
        _recruiterRepo = recruiterRepo;
        _emailHelper = emailHelper;
        _adminRepo = adminRepo;
        _notificationService = notificationService;
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

    public async Task<Article> CreateArticleAsync(int recruiterId, string title, string content, string thumbnailUrl, string actionType = "publish")
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
            Status = string.Equals(actionType, "draft", StringComparison.OrdinalIgnoreCase) ? "DRAFT" : "PUBLISHED",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // Not auto-assigning moderator because any moderator can handle articles

        return await _articleRepo.CreateAsync(article);
    }

    public async Task UpdateArticleAsync(int recruiterId, int articleId, string title, string content, string thumbnailUrl, string actionType = "publish")
    {
        var recruiter = await _recruiterRepo.GetProfileAsync(recruiterId);
        
        if (recruiter == null || recruiter.CompanyId == null)
        {
            throw new InvalidOperationException("Tài khoản của bạn chưa được liên kết với công ty nào.");
        }

        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null || article.CompanyId != recruiter.CompanyId)
            throw new InvalidOperationException("Article not found or unauthorized.");

        bool notifyMods = false;
        
        if (string.Equals(actionType, "draft", StringComparison.OrdinalIgnoreCase))
        {
            article.Status = "DRAFT";
            article.RejectReason = null;
        }
        else
        {
            if (!string.IsNullOrEmpty(article.RejectReason))
            {
                // Nếu bài từng bị từ chối, đưa về PENDING chờ duyệt lại
                article.Status = "PENDING";
                article.RejectReason = null;
                notifyMods = true; // Báo cho mod duyệt lại
            }
            else
            {
                // Trạng thái bình thường thì tự động PUBLISHED
                article.Status = "PUBLISHED";
            }
        }

        article.Title = title;
        article.Slug = GenerateSlug(title);
        article.Content = content;
        article.ThumbnailUrl = thumbnailUrl;
        article.UpdatedAt = DateTime.Now;

        await _articleRepo.UpdateAsync(article);

        if (notifyMods)
        {
            var mods = await _adminRepo.GetAllModeratorsAsync();
            foreach (var mod in mods)
            {
                try
                {
                    await _notificationService.SendNotificationAsync(
                        userId: mod.AdminId,
                        userType: "ADMIN",
                        title: "Bài báo cần duyệt lại",
                        message: $"Bài báo '{article.Title}' đã được chỉnh sửa và cần duyệt lại.",
                        type: "ARTICLE",
                        severity: "warning",
                        referenceId: article.ArticleId,
                        referenceType: "Article"
                    );
                }
                catch
                {
                    // Best effort
                }
            }
        }
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
        
        // Not auto-assigning moderator because any moderator can handle articles

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

        if (article.Status == "HIDDEN" && !string.IsNullOrEmpty(article.RejectReason))
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

        article.Status = "HIDDEN";
        article.RejectReason = reason;
        await _articleRepo.UpdateAsync(article);

        // Gửi thông báo cho Recruiter của công ty
        try
        {
            if (article.CompanyId.HasValue)
            {
                var recruiters = await _recruiterRepo.GetRecruitersByCompanyIdAsync(article.CompanyId.Value);
                foreach (var r in recruiters)
                {
                    var user = r.RecruiterNavigation;
                    if (user != null)
                    {
                        // In-app
                        await _notificationService.SendNotificationAsync(
                            user.UserId,
                            "RECRUITER",
                            "ARTICLE_REJECTED",
                            $"Bài viết '{article.Title}' đã bị ẩn/từ chối.",
                            $"/recruiter/articles"
                        );

                        // Email
                        if (user.EmailNotificationsEnabled)
                        {
                            string subject = "DevHub - Bài viết chưa được duyệt";
                            string body = $@"
                            <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                                <h2 style='color: #F44336;'>Bài viết chưa được duyệt</h2>
                                <p>Chào {r.FullName},</p>
                                <p>Bài viết <strong>{article.Title}</strong> của bạn bị ẩn/từ chối với lý do:</p>
                                <blockquote style='border-left: 4px solid #F44336; padding-left: 10px; color: #555;'>
                                    {reason}
                                </blockquote>
                                <p>Vui lòng cập nhật lại bài viết của bạn.</p>
                                <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                            </div>";
                            await _emailHelper.SendEmailAsync(user.Email, subject, body);
                        }
                    }
                }
            }
        }
        catch
        {
            // Bỏ qua lỗi
        }
    }

    public async Task ApproveArticleByModAsync(int articleId)
    {
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null)
            throw new InvalidOperationException("Article not found.");

        article.Status = "PUBLISHED";
        article.RejectReason = null;
        await _articleRepo.UpdateAsync(article);

        // Gửi thông báo cho Recruiter của công ty
        try
        {
            if (article.CompanyId.HasValue)
            {
                var recruiters = await _recruiterRepo.GetRecruitersByCompanyIdAsync(article.CompanyId.Value);
                foreach (var r in recruiters)
                {
                    var user = r.RecruiterNavigation;
                    if (user != null)
                    {
                        // In-app
                        await _notificationService.SendNotificationAsync(
                            user.UserId,
                            "RECRUITER",
                            "ARTICLE_APPROVED",
                            $"Bài viết '{article.Title}' đã được duyệt.",
                            $"/recruiter/articles"
                        );

                        // Email
                        if (user.EmailNotificationsEnabled)
                        {
                            string subject = "DevHub - Bài viết đã được duyệt";
                            string body = $@"
                            <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                                <h2 style='color: #4CAF50;'>Bài viết đã được duyệt!</h2>
                                <p>Chào {r.FullName},</p>
                                <p>Bài viết <strong>{article.Title}</strong> của bạn đã được phê duyệt.</p>
                                <p>Trân trọng,<br/>Đội ngũ DevHub</p>
                            </div>";
                            await _emailHelper.SendEmailAsync(user.Email, subject, body);
                        }
                    }
                }
            }
        }
        catch
        {
            // Bỏ qua lỗi
        }
    }

    public async Task DeleteArticleByModAsync(int articleId, string reason)
    {
        var article = await _articleRepo.GetByIdAsync(articleId);
        if (article == null)
            throw new InvalidOperationException("Article not found.");

        await _articleRepo.DeleteAsync(articleId);
    }
}

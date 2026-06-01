using DevHub.Repositories.Interfaces;
using DevHub.Data;
using DevHub.Models;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;
public class RecruiterJobPostRepository : IRecruiterJobPostRepository
{
    private readonly ItrecruitmentDbContext _context;

    public RecruiterJobPostRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<JobPost> CreateJobPostAndDecrementQuotaAsync(JobPost jobPost, int packageHistoryId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var package = await _context.RecruiterPackageHistories.FindAsync(packageHistoryId)
                ?? throw new InvalidOperationException("Tài khoản đã hết lượt đăng bài hoặc gói dịch vụ hết hạn.");

            if (package.PostsRemaining <= 0)
                throw new InvalidOperationException("Tài khoản đã hết lượt đăng bài hoặc gói dịch vụ hết hạn.");

            package.PostsRemaining = Math.Max(0, package.PostsRemaining - 1);

            jobPost.CreatedAt = DateTime.UtcNow;
            _context.JobPosts.Add(jobPost);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return jobPost;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task NotifyModeratorsAsync(string title, string message, string referenceType, int referenceId)
    {
        var moderators = await _context.UserAccounts.Where(u => u.UserType == "MODERATOR" && u.IsActive == true).ToListAsync();
        foreach (var mod in moderators)
        {
            var n = new Notification
            {
                UserId = mod.UserId,
                UserType = "MODERATOR",
                Type = "JobPostPending",
                Title = title,
                Message = message,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                SeverityLevel = "info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(n);
        }
        await _context.SaveChangesAsync();
    }
}
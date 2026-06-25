//AnhPT-02/06/2026
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

    //Get job posts list of a specific recruiter, order by created date desc.
    public async Task<List<JobPost>> GetJobPostsByRecruiterAsync(int recruiterId)
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Include(j => j.Position)
            .Where(j => j.RecruiterId == recruiterId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<JobPost?> GetJobPostForEditAsync(int jobId, int recruiterId)
    {
        return await _context.JobPosts
            .AsNoTracking()
            .Include(j => j.Position)
            .Include(j => j.Teches)
            .FirstOrDefaultAsync(j => j.JobId == jobId && j.RecruiterId == recruiterId);
    }

    // Edit job post fields: sync tech stacks, other fields.
    public async Task UpdateJobPostAsync(JobPost source, List<CommonTechnology> techs, string newStatus)
    {
        var job = await _context.JobPosts
            .Include(j => j.Teches)
            .FirstOrDefaultAsync(j => j.JobId == source.JobId && j.RecruiterId == source.RecruiterId)
            ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

        job.Title = source.Title;
        job.PositionId = source.PositionId;
        job.Location = source.Location;
        job.Skill = source.Skill;
        job.WorkingModel = source.WorkingModel;
        job.SalaryMin = source.SalaryMin;
        job.SalaryMax = source.SalaryMax;
        job.ExperienceLevel = source.ExperienceLevel;
        job.Description = source.Description;
        job.Requirement = source.Requirement;
        job.Benefit = source.Benefit;
        job.HiringQuota = source.HiringQuota;
        job.Deadline = source.Deadline;

        // sync tech stacks
        job.Teches.Clear();
        foreach (var tech in techs)
            job.Teches.Add(tech);

        // Status back to ''pending'' after edit, need moderated again.
        job.Status = newStatus;
        job.RejectedReason = null;
        job.ApprovedAt = null;
        job.ModeratorId = null;

        await _context.SaveChangesAsync();
    }

    // Close an active post: set status to CLOSED (owned by the recruiter).
    public Task CloseJobPostAsync(int jobId, int recruiterId)
        => _context.JobPosts
            .Where(j => j.JobId == jobId && j.RecruiterId == recruiterId)
            .ExecuteUpdateAsync(s => s.SetProperty(j => j.Status, "CLOSED"));

    //Delete job post: cascade delete related applications, interviews, bookmarks, job_tech_stack.
    public async Task DeleteJobPostAsync(int jobId, int recruiterId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM interview WHERE application_id IN (SELECT application_id FROM application WHERE job_id = {jobId})");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM application WHERE job_id = {jobId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM bookmark WHERE job_id = {jobId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM job_tech_stack WHERE job_id = {jobId}");
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM job_post WHERE job_id = {jobId} AND recruiter_id = {recruiterId}");

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

//create notification for moderators when job post is created or updated.
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

    // Notify every candidate whose application on this job is still active (PENDING/APPROVED) that the
    // recruiter has updated the posting and it is awaiting re-review. Their application stays as-is.
    public async Task NotifyApplicantsOnJobEditAsync(int jobId, string jobTitle)
    {
        var targets = await _context.Applications
            .AsNoTracking()
            .Where(a => a.JobId == jobId
                     && a.Status != null
                     && (a.Status.ToUpper() == "PENDING" || a.Status.ToUpper() == "APPROVED"))
            .Select(a => new { a.CandidateId, a.ApplicationId })
            .ToListAsync();

        if (targets.Count == 0) return;

        var notifications = targets.Select(t => new Notification
        {
            UserId = t.CandidateId,
            UserType = "CANDIDATE",
            Type = "APPLICATION",
            Title = "Tin tuyển dụng đang được cập nhật",
            Message = $"Nhà tuyển dụng vừa cập nhật thông tin tin \"{jobTitle}\". " +
                      "Thông tin mới sẽ có hiệu lực sau khi được kiểm duyệt. Đơn ứng tuyển của bạn vẫn được giữ nguyên.",
            ReferenceType = "Application",
            ReferenceId = t.ApplicationId,
            SeverityLevel = "info",
            IsRead = false,
            CreatedAt = DateTime.Now
        }).ToList();

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();
    }
}
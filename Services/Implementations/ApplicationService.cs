using DevHub.Models;
using DevHub.Data;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Jobs;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly ItrecruitmentDbContext _context;

    public ApplicationService(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<ApplyJobDataViewModel?> GetApplyInfoAsync(int candidateId)
    {
        var candidate = await _context.Candidates
            .Include(c => c.CandidateNavigation) // For UserAccount email
            .FirstOrDefaultAsync(c => c.CandidateId == candidateId);

        if (candidate == null)
            return null;

        // Try to find default CV, or fallback to the latest CV
        var cv = await _context.Cvs
            .Where(c => c.CandidateId == candidateId)
            .OrderByDescending(c => c.IsDefault) // true (1) comes first, then false (0)
            .ThenByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        return new ApplyJobDataViewModel
        {
            FullName = candidate.FullName,
            Email = candidate.CandidateNavigation.Email,
            Phone = candidate.Phone ?? "",
            Address = candidate.Address ?? "",
            ProfileCompletion = candidate.ProfileCompletion ?? 0,
            DefaultCvId = cv?.CvId,
            DefaultCvTitle = cv?.Title ?? "Chưa có CV"
        };
    }

    public async Task<(bool Success, string Message)> ApplyForJobAsync(int candidateId, SubmitApplyViewModel model)
    {
        var candidate = await _context.Candidates.FindAsync(candidateId);
        if (candidate == null)
            return (false, "Không tìm thấy thông tin ứng viên.");

        // Kiểm tra các trường bắt buộc (defence in depth)
        var missingFields = new List<string>();
        if (string.IsNullOrWhiteSpace(candidate.FullName))
            missingFields.Add("Họ và tên");
        if (string.IsNullOrWhiteSpace(candidate.Phone))
            missingFields.Add("Số điện thoại");
        if (missingFields.Any())
            return (false, $"Vui lòng cập nhật thông tin còn thiếu trước khi ứng tuyển: {string.Join(", ", missingFields)}.");

        if ((candidate.ProfileCompletion ?? 0) < 70)
            return (false, "Hồ sơ của bạn chưa đạt 70%. Vui lòng cập nhật hồ sơ trước khi ứng tuyển.");

        var cv = await _context.Cvs
            .Where(c => c.CandidateId == candidateId)
            .OrderByDescending(c => c.IsDefault)
            .ThenByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (cv == null)
            return (false, "Bạn chưa có CV trong hệ thống. Vui lòng tải lên CV trước.");

        var job = await _context.JobPosts.FindAsync(model.JobId);
        if (job == null)
            return (false, "Công việc không tồn tại.");

        if (job.Deadline.HasValue && job.Deadline.Value < DateOnly.FromDateTime(DateTime.Now))
            return (false, "Công việc này đã hết hạn ứng tuyển.");

        var existingApply = await _context.Applications
            .AnyAsync(a => a.CandidateId == candidateId && a.JobId == model.JobId);

        if (existingApply)
            return (false, "Bạn đã ứng tuyển công việc này rồi.");

        var application = new Application
        {
            JobId = model.JobId,
            CandidateId = candidateId,
            CvId = cv.CvId,
            CoverLetter = model.CoverLetter,
            Status = "PENDING",
            AppliedAt = DateTime.Now
        };

        _context.Applications.Add(application);

        // Update application count
        job.ApplicationCount = (job.ApplicationCount ?? 0) + 1;

        await _context.SaveChangesAsync();

        return (true, "Ứng tuyển thành công!");
    }

    public async Task<DevHub.ViewModels.Candidate.AppliedJobPageViewModel> GetPagedAppliedAsync(
        int candidateId, int page, int pageSize,
        string? keyword, string? timeRange, string? status)
    {
        var query = _context.Applications
            .Include(a => a.Job)
                .ThenInclude(j => j.Company)
            .Include(a => a.Job)
                .ThenInclude(j => j.Teches)
            .Where(a => a.CandidateId == candidateId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(a => 
                (a.Job.Title != null && a.Job.Title.ToLower().Contains(lowerKeyword)) ||
                (a.Job.Company.CompanyName != null && a.Job.Company.CompanyName.ToLower().Contains(lowerKeyword)));
        }

        if (!string.IsNullOrWhiteSpace(timeRange) && int.TryParse(timeRange, out int days))
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            query = query.Where(a => a.AppliedAt >= cutoffDate);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        int totalCount = await query.CountAsync();

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = applications.Select(a => new DevHub.ViewModels.Candidate.AppliedJobItemViewModel
        {
            ApplicationId = a.ApplicationId,
            JobId = a.JobId,
            JobTitle = a.Job.Title ?? "",
            CompanyName = a.Job.Company.CompanyName ?? "",
            CompanyLogoUrl = a.Job.Company.CompanyLogoUrl,
            Location = a.Job.Location,
            WorkingModel = a.Job.WorkingModel,
            ExperienceLevel = a.Job.ExperienceLevel,
            SalaryMin = a.Job.SalaryMin,
            SalaryMax = a.Job.SalaryMax,
            Deadline = a.Job.Deadline,
            TechNames = a.Job.Teches.Select(t => t.TechName).ToList(),
            Status = a.Status,
            AppliedAt = a.AppliedAt
        }).ToList();

        return new DevHub.ViewModels.Candidate.AppliedJobPageViewModel
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            SearchKeyword = keyword,
            FilterTimeRange = timeRange,
            FilterStatus = status
        };
    }

    public async Task<(string Status, DateTime? AppliedAt, List<InterviewInfo> Interviews)?> GetApplicationStatusAsync(
        int candidateId, int jobId)
    {
        var application = await _context.Applications
            .Include(a => a.Interviews)
            .FirstOrDefaultAsync(a => a.CandidateId == candidateId && a.JobId == jobId);

        if (application == null)
            return null;

        var interviews = application.Interviews
            .Select(i => new InterviewInfo(
                i.ScheduledTime,
                i.MeetingLink,
                i.Location,
                i.Status,
                i.InterviewType,
                i.Notes))
            .ToList();

        return (application.Status ?? "PENDING", application.AppliedAt, interviews);
    }
}

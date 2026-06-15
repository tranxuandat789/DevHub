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
}

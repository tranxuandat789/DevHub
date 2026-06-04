//AnhPT-03/06/2026
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
namespace DevHub.Services.Implementations;
public class RecruiterJobPostService : IRecruiterJobPostService
{
private const int MinProfileCompletion = 90;

    private readonly ICommonTechnologyRepository _techRepo;
    private readonly ICommonJobPositionRepository _positionRepo;
    private readonly IRecruiterPackageHistoryRepository _packageRepo;
    private readonly IRecruiterRepository _profileRepo;
    private readonly IRecruiterJobPostRepository _jobPostRepo;

    public RecruiterJobPostService(
        ICommonTechnologyRepository techRepo,
        ICommonJobPositionRepository positionRepo,
        IRecruiterPackageHistoryRepository packageRepo,
        IRecruiterRepository profileRepo,
        IRecruiterJobPostRepository jobPostRepo)
    {
        _techRepo = techRepo;
        _positionRepo = positionRepo;
        _packageRepo = packageRepo;
        _profileRepo = profileRepo;
        _jobPostRepo = jobPostRepo;
    }

    public async Task<(bool CanPost, bool HasActivePackage, int PostsRemaining, int ProfileCompletion)> GetActivePackageInfoAsync(int recruiterId)
    {
        var recruiter = await _profileRepo.GetProfileAsync(recruiterId);
        int completion = recruiter?.ProfileCompletion ?? 0;

        var package = await _packageRepo.GetActivePackageForRecruiterAsync(recruiterId);
        if (package == null)
            return (completion >= MinProfileCompletion, false, 0, completion);

        bool canPost = completion >= MinProfileCompletion && package.PostsRemaining > 0;
        return (canPost, true, package.PostsRemaining, completion);
    }

    public Task<List<JobPost>> GetJobPostsByRecruiterAsync(int recruiterId)
        => _jobPostRepo.GetJobPostsByRecruiterAsync(recruiterId);

    public async Task<int> CreateJobPostAsync(int recruiterId, JobPostCreateViewModel dto)
    {
        var recruiter = await _profileRepo.GetProfileAsync(recruiterId);
        if (recruiter == null)
            throw new KeyNotFoundException("Không tìm thấy thông tin nhà tuyển dụng.");

        if ((recruiter.ProfileCompletion ?? 0) < MinProfileCompletion)
            throw new InvalidOperationException("Bạn cần hoàn thành đủ mục thông tin công ty");

        var package = await _packageRepo.GetActivePackageForRecruiterAsync(recruiterId);
        if (package == null || package.PostsRemaining <= 0)
            throw new InvalidOperationException("Tài khoản đã hết lượt đăng bài hoặc gói dịch vụ hết hạn.");

        var position = await _positionRepo.GetByIdAsync(dto.PositionId);
        if (position == null || position.IsActive == false)
            throw new InvalidOperationException("Vị trí công việc được chọn không tồn tại hoặc đã bị ẩn.");

        var uniqueTechIds = dto.TechnologyIds.Distinct().ToList();
        if (uniqueTechIds.Count == 0)
            throw new InvalidOperationException("Vui lòng chọn ít nhất một công nghệ từ danh sách.");

        var techEntities = await _techRepo.GetByIdsAsync(uniqueTechIds);
        if (techEntities.Count != uniqueTechIds.Count)
            throw new InvalidOperationException("Danh sách công nghệ được chọn không hợp lệ.");

        if (dto.SalaryMax < dto.SalaryMin)
            throw new InvalidOperationException("Mức lương tối đa phải lớn hơn hoặc bằng mức lương tối thiểu.");

        var job = new JobPost
        {
            RecruiterId = recruiterId,
            PositionId = dto.PositionId,
            RecruiterPackageHistoryId = package.Id,
            Title = dto.Title,
            Description = dto.Description,
            Requirement = dto.Requirement,
            Benefit = dto.Benefit,
            Skill = dto.Skill,
            ExperienceLevel = dto.ExperienceLevel,
            Location = dto.Location,
            WorkingModel = dto.WorkingModel,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            HiringQuota = dto.HiringQuota,
            Deadline = dto.Deadline,
            Status = "Pending",
            PriorityScore = package.Service?.PriorityPush ?? 0,
        };

        foreach (var tech in techEntities)
            job.Teches.Add(tech);

        var createdJob = await _jobPostRepo.CreateJobPostAndDecrementQuotaAsync(job, package.Id);

        try
        {
            await _jobPostRepo.NotifyModeratorsAsync(
                "Bài đăng tuyển dụng mới chờ duyệt",
                $"Nhà tuyển dụng {recruiter.CompanyName} đã đăng bài '{job.Title}' và đang chờ kiểm duyệt.",
                "JobPost",
                createdJob.JobId);
        }
        catch
        {
            // Notification failure must not roll back the created job post.
        }

        return createdJob.JobId;
    }

}
    
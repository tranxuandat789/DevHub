//AnhPT-03/06/2026
using System.Text.RegularExpressions;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
namespace DevHub.Services.Implementations;
public class RecruiterJobPostService : IRecruiterJobPostService
{
    private const int MinProfileCompletion = 90;

    // Editable: APPROVED, REJECTED. PENDING, CLOSED posts are view-only.
    private static readonly string[] EditableStatuses = { "approved", "rejected" };
    // Deletable: only Rejected or Closed posts.
    private static readonly string[] DeletableStatuses = { "rejected", "closed" };

    //standardize to normal case
    private static string Canon(string? s) => (s ?? "").Trim().ToLowerInvariant();

    // Validating search string: trim, lowercase, and collapse multiple spaces to one.
    // Search 'Senior   springboot   dev' could match 'senior springboot dev'.
    private static string NormalizeText(string? s) =>
        Regex.Replace((s ?? "").Trim().ToLowerInvariant(), @"\s+", " ");

    //Repository Instance
    private readonly ICommonTechnologyRepository _techRepo;
    private readonly ICommonJobPositionRepository _positionRepo;
    private readonly IRecruiterPackageHistoryRepository _packageRepo;
    private readonly IRecruiterRepository _profileRepo;
    private readonly IRecruiterJobPostRepository _jobPostRepo;

    //Constructor Injection
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

    // Compute posting eligibility: profile completeness + active package with remaining quota.
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

    // Return all posts owned by the recruiter (used by dashboard/list helpers).
    public Task<List<JobPost>> GetJobPostsByRecruiterAsync(int recruiterId)
        => _jobPostRepo.GetJobPostsByRecruiterAsync(recruiterId);

    // Validate preconditions + input, create the post with status = PENDING), decrement quota, notify moderators.
    public async Task<int> CreateJobPostAsync(int recruiterId, JobPostCreateViewModel vm)
    {
        //get recruiter profile to check profile completeness
        var recruiter = await _profileRepo.GetProfileAsync(recruiterId);
        if (recruiter == null)
            throw new KeyNotFoundException("Không tìm thấy thông tin nhà tuyển dụng.");

        //Unsatisfied profile completeness.
        if ((recruiter.ProfileCompletion ?? 0) < MinProfileCompletion)
            throw new InvalidOperationException("Bạn cần hoàn thành đủ mục thông tin công ty");

        //Track if the package is valid and has remaining posts.
        var package = await _packageRepo.GetActivePackageForRecruiterAsync(recruiterId);
        if (package == null || package.PostsRemaining <= 0)
            throw new InvalidOperationException("Tài khoản đã hết lượt đăng bài hoặc gói dịch vụ hết hạn.");

        //Binding fixed input for job position. 
        var position = await _positionRepo.GetByIdAsync(vm.PositionId);
        if (position == null)
            throw new InvalidOperationException("Vị trí công việc được chọn không tồn tại hoặc đã bị ẩn.");

        //Binding fixed input for tech-stack. 
        var uniqueTechIds = vm.TechnologyIds.Distinct().ToList();
        if (uniqueTechIds.Count == 0)
            throw new InvalidOperationException("Vui lòng chọn ít nhất một công nghệ từ danh sách.");

        // Validate that all provided tech IDs exist in the database.
        var techEntities = await _techRepo.GetByIdsAsync(uniqueTechIds);
        if (techEntities.Count != uniqueTechIds.Count)
            throw new InvalidOperationException("Danh sách công nghệ được chọn không hợp lệ.");

        // Validate salary range.
        if (vm.SalaryMax < vm.SalaryMin)
            throw new InvalidOperationException("Mức lương tối đa phải lớn hơn hoặc bằng mức lương tối thiểu.");

        var job = new JobPost
        {
            RecruiterId = recruiterId,
            PositionId = vm.PositionId,
            RecruiterPackageHistoryId = package.Id,
            Title = vm.Title,
            Description = vm.Description,
            Requirement = vm.Requirement,
            Benefit = vm.Benefit,
            Skill = vm.Skill,
            ExperienceLevel = vm.ExperienceLevel,
            Location = vm.Location,
            WorkingModel = vm.WorkingModel,
            SalaryMin = vm.SalaryMin,
            SalaryMax = vm.SalaryMax,
            HiringQuota = vm.HiringQuota,
            Deadline = vm.Deadline,
            Status = "PENDING",
            PriorityScore = package.Service?.PriorityPush ?? 0,
        };

        //Add list of tech stack to the job post
        foreach (var tech in techEntities)
            job.Teches.Add(tech);

        var createdJob = await _jobPostRepo.CreateJobPostAndDecrementQuotaAsync(job, package.Id);

        //send notification to moderators about new pending job post.
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

    //Recruiter get Job posts list which they have posted, default: oder by date time.
    //Load all of the recruiter's posts, count per status, with pagination and search box
    public async Task<JobPostManageViewModel> GetManagedJobPostsAsync(int recruiterId, string? keyword, string? status, int page, int pageSize)
    {
        //Init value is list of all Recruiter's job posts.
        var all = await _jobPostRepo.GetJobPostsByRecruiterAsync(recruiterId);

        var vm = new JobPostManageViewModel
        {
            Keyword = keyword,
            Status = string.IsNullOrWhiteSpace(status) ? null : Canon(status),
            PageSize = pageSize <= 0 ? 10 : pageSize, // default page size = 10.
            HasAnyPost = all.Count > 0,
            CountAll = all.Count,
            CountPending = all.Count(j => Canon(j.Status) == "pending"),
            CountApproved = all.Count(j => Canon(j.Status) == "approved"),
            CountRejected = all.Count(j => Canon(j.Status) == "rejected"),
            CountClosed = all.Count(j => Canon(j.Status) == "closed"),
        };
        //List of jobposts for filtering and pagination.
        IEnumerable<JobPost> filtered = all;

        // standardize status filter: trim, lowercase, and ignore empty, then use linq to add filter.
        if (!string.IsNullOrWhiteSpace(vm.Status))
            filtered = filtered.Where(j => Canon(j.Status) == vm.Status);

        var needle = NormalizeText(keyword);
        if (needle.Length > 0)
            filtered = filtered.Where(j => NormalizeText(j.Title).Contains(needle));

        var matched = filtered.ToList();
        vm.TotalCount = matched.Count;
        vm.TotalPages = Math.Max(1, (int)Math.Ceiling(matched.Count / (double)vm.PageSize));
        vm.Page = Math.Min(Math.Max(1, page), vm.TotalPages);

        vm.Items = matched
            .Skip((vm.Page - 1) * vm.PageSize)
            .Take(vm.PageSize)
            .ToList();

        return vm;
    }

    // Load a post (any status) owned by the recruiter for the read-only detail view.
    public Task<JobPost?> GetJobPostDetailAsync(int recruiterId, int jobId)
        => _jobPostRepo.GetJobPostForEditAsync(jobId, recruiterId);

    // Return the post only if it is owned by the recruiter & its status allows editing.
    public async Task<JobPost?> GetEditableJobPostAsync(int recruiterId, int jobId)
    {
        var job = await _jobPostRepo.GetJobPostForEditAsync(jobId, recruiterId);
        if (job == null) return null;
        return EditableStatuses.Contains(Canon(job.Status)) ? job : null;
    }

    // Validate ownership/editable status + input, persist changes, then resubmit the post
    // as PENDING for moderator re-review 
    public async Task UpdateJobPostAsync(int recruiterId, int jobId, JobPostCreateViewModel vm)
    {
        //Get a specific job post
        var existing = await _jobPostRepo.GetJobPostForEditAsync(jobId, recruiterId)
            ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

        //Check if selected jobpost is editable?
        if (!EditableStatuses.Contains(Canon(existing.Status)))
            throw new InvalidOperationException("Không thể chỉnh sửa tin đã đóng.");

        //check if selected jobposition is invalid
        var position = await _positionRepo.GetByIdAsync(vm.PositionId);
        if (position == null)
            throw new InvalidOperationException("Vị trí công việc được chọn không tồn tại hoặc đã bị ẩn.");

        //check if no technolagies are selected
        var uniqueTechIds = vm.TechnologyIds.Distinct().ToList();
        if (uniqueTechIds.Count == 0)
            throw new InvalidOperationException("Vui lòng chọn ít nhất một công nghệ từ danh sách.");

        //check if selected technolagies are invalid
        var techEntities = await _techRepo.GetByIdsAsync(uniqueTechIds);
        if (techEntities.Count != uniqueTechIds.Count)
            throw new InvalidOperationException("Danh sách công nghệ được chọn không hợp lệ.");

        //check if min salary > max salary
        if (vm.SalaryMax < vm.SalaryMin)
            throw new InvalidOperationException("Mức lương tối đa phải lớn hơn hoặc bằng mức lương tối thiểu.");

        var updatedJP = new JobPost
        {
            JobId = jobId,
            RecruiterId = recruiterId,
            Title = vm.Title,
            PositionId = vm.PositionId,
            Location = vm.Location,
            Skill = vm.Skill,
            WorkingModel = vm.WorkingModel,
            SalaryMin = vm.SalaryMin,
            SalaryMax = vm.SalaryMax,
            ExperienceLevel = vm.ExperienceLevel,
            Description = vm.Description,
            Requirement = vm.Requirement,
            Benefit = vm.Benefit,
            HiringQuota = vm.HiringQuota,
            Deadline = vm.Deadline,
        };

        // Both Approved and Rejected posts return to PENDING for moderator re-review.
        await _jobPostRepo.UpdateJobPostAsync(updatedJP, techEntities, "PENDING");

        try
        {
            await _jobPostRepo.NotifyModeratorsAsync(
                "Bài đăng tuyển dụng được chỉnh sửa, chờ duyệt lại",
                $"Tin '{updatedJP.Title}' vừa được cập nhật và đang chờ kiểm duyệt lại.",
                "JobPost",
                jobId);
        }
        catch
        {
            // Notification failure must not roll back the update.
        }
    }

    // Validate ownership + deletable status, then permanently purge the post.
    public async Task DeleteJobPostAsync(int recruiterId, int jobId)
    {
        var job = await _jobPostRepo.GetJobPostForEditAsync(jobId, recruiterId)
            ?? throw new KeyNotFoundException("Không tìm thấy tin tuyển dụng.");

        if (!DeletableStatuses.Contains(Canon(job.Status)))
            throw new InvalidOperationException("Chỉ có thể xóa tin ở trạng thái Bị từ chối hoặc Đã đóng.");

        await _jobPostRepo.DeleteJobPostAsync(jobId, recruiterId);
    }

}
    
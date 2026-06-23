//AnhPT-18/06/2026
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Implementations
{
    public class RecruiterApplicationService : IRecruiterApplicationService
    {
        private const int PageSize = 10;
        private readonly IRecruiterApplicationRepository _repo;

        public RecruiterApplicationService(IRecruiterApplicationRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApplicantListViewModel?> GetJobApplicantsAsync(int recruiterId, int jobId, ApplicantFilter filter)
        {
            // Precondition UC-14: job must belong to the recruiter and be APPROVED.
            var job = await _repo.GetOwnedApprovedJobAsync(jobId, recruiterId);
            if (job == null) return null;

            var vm = await BuildListAsync(recruiterId, jobId, filter, isCrossJob: false);
            vm.JobId = job.JobId;
            vm.JobTitle = job.Title;
            vm.JobStatus = job.Status;
            return vm;
        }

        public async Task<ApplicantListViewModel> GetAllApplicantsAsync(int recruiterId, ApplicantFilter filter)
            => await BuildListAsync(recruiterId, jobId: null, filter, isCrossJob: true);

        private async Task<ApplicantListViewModel> BuildListAsync(int recruiterId, int? jobId, ApplicantFilter filter, bool isCrossJob)
        {
            int page = filter.Page < 1 ? 1 : filter.Page;

            var (items, total) = await _repo.GetApplicantsAsync(recruiterId, jobId, filter, page, PageSize);
            var counts = await _repo.CountByStatusAsync(recruiterId, jobId);

            var vm = new ApplicantListViewModel
            {
                IsCrossJob = isCrossJob,
                JobId = jobId,
                Items = items.Select(a => new ApplicantItem
                {
                    ApplicationId = a.ApplicationId,
                    CandidateId = a.CandidateId,
                    FullName = a.Candidate.FullName,
                    AvatarUrl = a.Candidate.ImageUrl,
                    ExperienceYears = a.Candidate.ExperienceYears,
                    PreferredLocation = a.Candidate.PreferredLocation,
                    AppliedAt = a.AppliedAt,
                    Status = (a.Status ?? "PENDING").ToUpper(),
                    TopSkills = a.Candidate.CandidateSkills
                        .Where(s => s.Tech != null)
                        .Select(s => s.Tech.TechName)
                        .Take(5)
                        .ToList(),
                    JobTitle = isCrossJob ? a.Job.Title : null
                }).ToList(),
                CountAll = counts.All,
                CountPending = counts.Pending,
                CountApproved = counts.Approved,
                CountRejected = counts.Rejected,
                Page = page,
                PageSize = PageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)PageSize),
                TechOptions = await _repo.GetActiveTechOptionsAsync(),
                LocationOptions = await _repo.GetCandidateLocationOptionsAsync(recruiterId, jobId),
                Filter = filter
            };

            if (isCrossJob)
                vm.PositionOptions = await _repo.GetActivePositionOptionsAsync();

            return vm;
        }

        public async Task<CandidateProfileViewModel?> GetCandidateProfileAsync(int recruiterId, int applicationId)
        {
            var a = await _repo.GetApplicationDetailAsync(applicationId, recruiterId);
            if (a == null) return null;

            return new CandidateProfileViewModel
            {
                ApplicationId = a.ApplicationId,
                CandidateId = a.CandidateId,
                JobId = a.JobId,
                JobTitle = a.Job?.Title ?? "",
                FullName = a.Candidate.FullName,
                AvatarUrl = a.Candidate.ImageUrl,
                Email = a.Candidate.CandidateNavigation?.Email,
                Gender = a.Candidate.Gender,
                Phone = a.Candidate.Phone,
                Address = a.Candidate.Address,
                PreferredLocation = a.Candidate.PreferredLocation,
                SocialMediaUrl = a.Candidate.SocialMediaUrl,
                ExperienceYears = a.Candidate.ExperienceYears,
                ExpectedSalaryMin = a.Candidate.ExpectedSalaryMin,
                ExpectedSalaryMax = a.Candidate.ExpectedSalaryMax,
                Skills = a.Candidate.CandidateSkills
                    .Where(s => s.Tech != null)
                    .Select(s => (s.Tech.TechName, s.Level))
                    .ToList(),
                CvTitle = a.Cv?.Title,
                CvUrl = a.Cv?.CvUrl,
                CoverLetter = a.CoverLetter,
                Status = (a.Status ?? "PENDING").ToUpper(),
                AppliedAt = a.AppliedAt
            };
        }

        public async Task<(bool Success, string Message)> ApproveAsync(int recruiterId, int applicationId)
        {
            var app = await _repo.UpdateStatusIfPendingAsync(applicationId, recruiterId, "APPROVED");
            if (app == null)
                return (false, "Đơn ứng tuyển không tồn tại hoặc đã được xử lý.");

            await _repo.CreateCandidateNotificationAsync(
                app.CandidateId,
                "Đơn ứng tuyển được chấp nhận",
                $"Hồ sơ của bạn cho vị trí \"{app.Job?.Title}\" đã được nhà tuyển dụng chấp nhận. Vui lòng chờ lịch phỏng vấn.",
                "success",
                app.ApplicationId);

            // Approving unlocks Interview Scheduling for this application (gated in the Interview flow).
            return (true, "Đã duyệt ứng viên. Bạn có thể lên lịch phỏng vấn.");
        }

        public async Task<(bool Success, string Message)> RejectAsync(int recruiterId, int applicationId)
        {
            var app = await _repo.UpdateStatusIfPendingAsync(applicationId, recruiterId, "REJECTED");
            if (app == null)
                return (false, "Đơn ứng tuyển không tồn tại hoặc đã được xử lý.");

            await _repo.CreateCandidateNotificationAsync(
                app.CandidateId,
                "Đơn ứng tuyển bị từ chối",
                $"Rất tiếc, hồ sơ của bạn cho vị trí \"{app.Job?.Title}\" chưa phù hợp ở thời điểm này.",
                "warning",
                app.ApplicationId);

            return (true, "Đã từ chối ứng viên.");
        }
    }
}

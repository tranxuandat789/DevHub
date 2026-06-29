//AnhPT-18/06/2026
using DevHub.Helpers;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.Extensions.Logging;

namespace DevHub.Services.Implementations
{
    public class RecruiterApplicationService : IRecruiterApplicationService
    {
        private const int PageSize = 10;
        private readonly IRecruiterApplicationRepository _repo;
        private readonly ILogger<RecruiterApplicationService> _logger;
        private readonly EmailHelper _emailHelper;

        public RecruiterApplicationService(IRecruiterApplicationRepository repo, ILogger<RecruiterApplicationService> logger, EmailHelper emailHelper)
        {
            _repo = repo;
            _logger = logger;
            _emailHelper = emailHelper;
        }

        public async Task<ApplicantListViewModel?> GetJobApplicantsAsync(int recruiterId, int jobId, ApplicantFilter filter)
        {
            //Recruiter only can access jobs they possesed and status has to be APPROVED or CLOSED.
            var job = await _repo.GetOwnedJobAsync(jobId, recruiterId);
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
            var counts = await _repo.CountByStatusAsync(recruiterId, jobId, filter);

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
                CountHired = counts.Hired,
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
                JobStatus = (a.Job?.Status ?? "").ToUpper(),
                AppliedAt = a.AppliedAt,
                TotalApplicationsAtCompany = await _repo.CountApplicationsAtCompanyAsync(a.CandidateId, recruiterId)
            };
        }

        public async Task<CandidateProfileHistoryViewModel?> GetCandidateProfileHistoryAsync(int recruiterId, int candidateId)
            => await _repo.GetCandidateProfileHistoryAsync(recruiterId, candidateId);

        public async Task<(bool Success, string Message)> ApproveAsync(int recruiterId, int applicationId)
        {
            // Freeze gate: block approve/reject while the job is pending re-review (recruiter just edited it).
            var (appCheck, jobStatus) = await _repo.GetApplicationWithJobStatusAsync(applicationId, recruiterId);
            if (appCheck == null)
                return (false, "Đơn ứng tuyển không tồn tại hoặc bạn không có quyền truy cập.");
            if ((jobStatus ?? "").ToUpper() == "PENDING")
                return (false, "Tin tuyển dụng đang chờ kiểm duyệt lại. Vui lòng chờ sau khi tin được duyệt.");

            var app = await _repo.UpdateStatusIfPendingAsync(applicationId, recruiterId, "APPROVED");
            if (app == null)
                return (false, "Đơn ứng tuyển không tồn tại hoặc đã được xử lý.");

            // Notification is best-effort: the status change is already committed, so a notification
            // failure must NOT roll back / fail the approve action.
            try
            {
                await _repo.CreateCandidateNotificationAsync(
                    app.CandidateId,
                    "Đơn ứng tuyển được chấp nhận",
                    $"Hồ sơ của bạn cho vị trí \"{app.Job?.Title}\" đã được nhà tuyển dụng chấp nhận. Vui lòng chờ lịch phỏng vấn.",
                    "success",
                    app.ApplicationId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create approval notification for application {ApplicationId}", app.ApplicationId);
            }

            // Best-effort email to the candidate (won't fail the approve action).
            await SendCandidateDecisionEmailSafeAsync(app.CandidateId, isApproved: true, jobTitle: app.Job?.Title);

            // Approving unlocks Interview Scheduling for this application (gated in the Interview flow).
            return (true, "Đã duyệt ứng viên. Bạn có thể lên lịch phỏng vấn.");
        }

        public async Task<(bool Success, string Message)> RejectAsync(int recruiterId, int applicationId)
        {
            // Freeze gate (see ApproveAsync).
            var (appCheck, jobStatus) = await _repo.GetApplicationWithJobStatusAsync(applicationId, recruiterId);
            if (appCheck == null)
                return (false, "Đơn ứng tuyển không tồn tại hoặc bạn không có quyền truy cập.");
            if ((jobStatus ?? "").ToUpper() == "PENDING")
                return (false, "Tin tuyển dụng đang chờ kiểm duyệt lại. Vui lòng chờ sau khi tin được duyệt.");

            var app = await _repo.UpdateStatusIfPendingAsync(applicationId, recruiterId, "REJECTED");
            if (app == null)
                return (false, "Đơn ứng tuyển không tồn tại hoặc đã được xử lý.");

            // Best-effort notification (see ApproveAsync).
            try
            {
                await _repo.CreateCandidateNotificationAsync(
                    app.CandidateId,
                    "Đơn ứng tuyển bị từ chối",
                    $"Rất tiếc, hồ sơ của bạn cho vị trí \"{app.Job?.Title}\" chưa phù hợp ở thời điểm này.",
                    "warning",
                    app.ApplicationId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create rejection notification for application {ApplicationId}", app.ApplicationId);
            }

            // Best-effort email to the candidate.
            await SendCandidateDecisionEmailSafeAsync(app.CandidateId, isApproved: false, jobTitle: app.Job?.Title);

            return (true, "Đã từ chối ứng viên.");
        }

        // Sends the approve/reject email to the candidate. Best-effort: never throws to the caller.
        private async Task SendCandidateDecisionEmailSafeAsync(int candidateId, bool isApproved, string? jobTitle)
        {
            try
            {
                var (email, fullName) = await _repo.GetCandidateContactAsync(candidateId);
                if (string.IsNullOrWhiteSpace(email)) return;

                var (subject, body) = isApproved
                    ? BuildApprovedEmail(fullName, jobTitle)
                    : BuildRejectedEmail(fullName, jobTitle);

                await _emailHelper.SendEmailAsync(email!, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send candidate decision email (candidate {CandidateId}, approved={Approved})", candidateId, isApproved);
            }
        }

        private static (string Subject, string Body) BuildApprovedEmail(string fullName, string? jobTitle)
        {
            var position = string.IsNullOrWhiteSpace(jobTitle) ? "vị trí bạn đã ứng tuyển" : jobTitle;
            var name = string.IsNullOrWhiteSpace(fullName) ? "bạn" : fullName;
            var subject = "Hồ sơ ứng tuyển được chấp nhận - DevHub";
            var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;
                        border: 1px solid #D6DDEB; border-radius: 8px;'>
                <h2 style='color: #4640DE; text-align: center;'>Chúc mừng! Hồ sơ của bạn được chấp nhận</h2>
                <p>Xin chào {name},</p>
                <p>Hồ sơ ứng tuyển của bạn cho vị trí <b>""{position}""</b> đã được nhà tuyển dụng <b>chấp nhận</b>.</p>
                <div style='text-align: center; margin: 28px 0;'>
                    <span style='display: inline-block; font-size: 18px; font-weight: bold; color: #16A34A;
                                 background: #ECFDF5; padding: 14px 28px; border-radius: 8px;
                                 border: 1px dashed #16A34A;'>✓ Hồ sơ đã được duyệt</span>
                </div>
                <p>Nhà tuyển dụng có thể sắp xếp lịch phỏng vấn với bạn trong thời gian tới. Vui lòng theo dõi thông báo và email để cập nhật lịch phỏng vấn.</p>
                <p style='color: #4640DE;'>Chúc bạn thành công trong các vòng tiếp theo!</p>
                <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
            </div>";
            return (subject, body);
        }

        private static (string Subject, string Body) BuildRejectedEmail(string fullName, string? jobTitle)
        {
            var position = string.IsNullOrWhiteSpace(jobTitle) ? "vị trí bạn đã ứng tuyển" : jobTitle;
            var name = string.IsNullOrWhiteSpace(fullName) ? "bạn" : fullName;
            var subject = "Kết quả ứng tuyển - DevHub";
            var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;
                        border: 1px solid #D6DDEB; border-radius: 8px;'>
                <h2 style='color: #4640DE; text-align: center;'>Kết quả ứng tuyển của bạn</h2>
                <p>Xin chào {name},</p>
                <p>Cảm ơn bạn đã quan tâm và ứng tuyển vị trí <b>""{position}""</b> tại DevHub.</p>
                <div style='text-align: center; margin: 28px 0;'>
                    <span style='display: inline-block; font-size: 16px; font-weight: bold; color: #B45309;
                                 background: #FFFBEB; padding: 14px 28px; border-radius: 8px;
                                 border: 1px dashed #F59E0B;'>Hồ sơ chưa được chọn ở vòng này</span>
                </div>
                <p>Rất tiếc, hồ sơ của bạn chưa phù hợp với vị trí này ở thời điểm hiện tại. Đây không phải là đánh giá về toàn bộ năng lực của bạn — hãy tiếp tục theo dõi và ứng tuyển các vị trí phù hợp khác trên DevHub.</p>
                <p style='color: #4640DE;'>Chúc bạn sớm tìm được công việc phù hợp!</p>
                <hr style='border: none; border-top: 1px solid #E5E5E5; margin: 20px 0;' />
                <p style='font-size: 12px; color: #888888; text-align: center;'>Hệ thống tuyển dụng DevHub</p>
            </div>";
            return (subject, body);
        }
    }
}

//AnhPT-18/06/2026
using DevHub.Models;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Repositories.Interfaces
{
    // Data access for the recruiter "View Applicants" / "Check Candidate Profile" feature (UC-14/15).
    public interface IRecruiterApplicationRepository
    {
        // Precondition: returns the job only if it belongs to the recruiter and is NOT PENDING/REJECTED
        // (i.e. it has been published at least once: APPROVED, CLOSED, FINISHED, EXPIRED).
        Task<JobPost?> GetOwnedJobAsync(int jobId, int recruiterId);

        // Applicant list. jobId == null => cross-job (all jobs owned by the recruiter).
        Task<(List<Application> Items, int TotalCount)> GetApplicantsAsync(
            int recruiterId, int? jobId, ApplicantFilter filter, int page, int pageSize);

        // Status counts for the tabs. jobId == null => cross-job.
        // Applies ALL active filters EXCEPT status, so tab counts match the filtered list.
        Task<(int All, int Pending, int Approved, int Hired, int Rejected, int Interviewing)> CountByStatusAsync(int recruiterId, int? jobId, ApplicantFilter filter);

        // Full application detail (candidate + skills + cv), scoped to the recruiter.
        Task<Application?> GetApplicationDetailAsync(int applicationId, int recruiterId);

        // Lightweight lookup returning the application + its parent job status, used to gate
        // approve/reject while the job is frozen (PENDING re-review).
        Task<(Application? App, string? JobStatus)> GetApplicationWithJobStatusAsync(int applicationId, int recruiterId);

        // Candidate-centric profile: candidate info + full application history at this recruiter's company.
        // Returns null when the candidate has no application to any of the recruiter's jobs (no access).
        Task<CandidateProfileHistoryViewModel?> GetCandidateProfileHistoryAsync(int recruiterId, int candidateId);

        // Count of applications this candidate submitted to the recruiter's jobs (for the "view full history" link).
        Task<int> CountApplicationsAtCompanyAsync(int candidateId, int recruiterId);

        // Approve/Reject: only updates when the application belongs to the recruiter and is PENDING.
        // Returns the updated application (for notification) or null when it was not eligible.
        Task<Application?> UpdateStatusIfPendingAsync(int applicationId, int recruiterId, string newStatus);

        // Hire: only updates when the application belongs to the recruiter and is APPROVED.
        Task<Application?> UpdateStatusIfApprovedToHiredAsync(int applicationId, int recruiterId);

        // Creates a notification addressed to a candidate.
        Task CreateCandidateNotificationAsync(int candidateId, string title, string message, string severity, int applicationId);

        // Candidate contact (email, full name, email notifications enabled) for sending emails.
        Task<(string? Email, string FullName, bool EmailNotificationsEnabled)> GetCandidateContactAsync(int candidateId);

        // Dropdown options (values are bound from these lists only).
        Task<List<CommonTechnology>> GetActiveTechOptionsAsync();
        Task<List<CommonJobPosition>> GetActivePositionOptionsAsync();
        Task<List<string>> GetCandidateLocationOptionsAsync(int recruiterId, int? jobId);
    }
}

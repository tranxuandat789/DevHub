//AnhPT-18/06/2026
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Interfaces
{
    // Business logic for the recruiter "View Applicants" / "Check Candidate Profile" feature (UC-14/15).
    public interface IRecruiterApplicationService
    {
        // UC-14 per-job. Returns null when the job does not belong to the recruiter or is not APPROVED.
        Task<ApplicantListViewModel?> GetJobApplicantsAsync(int recruiterId, int jobId, ApplicantFilter filter);

        // Cross-job (thin reuse layer).
        Task<ApplicantListViewModel> GetAllApplicantsAsync(int recruiterId, ApplicantFilter filter);

        // UC-15. Returns null when the application is not visible to the recruiter.
        Task<CandidateProfileViewModel?> GetCandidateProfileAsync(int recruiterId, int applicationId);

        // Candidate Profile (cross-application history). Returns null when the candidate has no
        // application to any of the recruiter's jobs.
        Task<CandidateProfileHistoryViewModel?> GetCandidateProfileHistoryAsync(int recruiterId, int candidateId);

        Task<(bool Success, string Message)> ApproveAsync(int recruiterId, int applicationId);
        Task<(bool Success, string Message)> RejectAsync(int recruiterId, int applicationId);
    }
}

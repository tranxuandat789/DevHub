namespace DevHub.Services.Interfaces;

using DevHub.ViewModels.Candidate;
using DevHub.ViewModels.Jobs;

public interface IApplicationService
{
    Task<ApplyJobDataViewModel?> GetApplyInfoAsync(int candidateId);
    Task<(bool Success, string Message)> ApplyForJobAsync(int candidateId, SubmitApplyViewModel model);
    
    Task<AppliedJobPageViewModel> GetPagedAppliedAsync(
        int candidateId, int page, int pageSize,
        string? keyword, string? timeRange, string? status);

    Task<(string Status, DateTime? AppliedAt, List<InterviewInfo> Interviews)?> GetApplicationStatusAsync(
        int candidateId, int jobId);
}

public record InterviewInfo(
    DateTime ScheduledTime,
    string? MeetingLink,
    string? Location,
    string? Status,
    string? InterviewType,
    string? Notes);

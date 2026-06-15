namespace DevHub.Services.Interfaces;

using DevHub.ViewModels.Jobs;

public interface IApplicationService
{
    Task<ApplyJobDataViewModel?> GetApplyInfoAsync(int candidateId);
    Task<(bool Success, string Message)> ApplyForJobAsync(int candidateId, SubmitApplyViewModel model);
}

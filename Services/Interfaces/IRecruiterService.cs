//AnhPT-01/06/2026
using DevHub.Models;
using DevHub.ViewModels.Recruiter;

namespace DevHub.Services.Interfaces;

public interface IRecruiterService
{
    Task<RecruiterProfileViewModel> GetProfileAsync(int recruiterId);
    Task UpdateProfileAsync(Recruiter recruiter, RecruiterProfileViewModel updateVm);
    Task RegisterCompanyProfileAsync(Recruiter existingRecruiter, RecruiterProfileViewModel updateVm);
    Task SendVerificationRequestAsync(int recruiterId);
    Task<bool> HasPendingVerificationRequestAsync(int recruiterId);
    Task ChangePasswordAsync(int recruiterId, RecruiterChangePasswordViewModel vm);
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces
{
    public interface ICompanyInvitationService
    {
        Task<CompanyInvitation> InviteMemberAsync(int companyId, string email, int invitedByRecruiterId);
        Task<CompanyInvitation?> ValidateTokenAsync(string token);
        Task<bool> AcceptInvitationAsync(string token, int newRecruiterId);
        Task<bool> CancelInvitationAsync(int invitationId, int companyId);
        Task<IEnumerable<CompanyInvitation>> GetPendingInvitationsAsync(int companyId);
    }
}

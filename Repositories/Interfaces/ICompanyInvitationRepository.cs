using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces
{
    public interface ICompanyInvitationRepository
    {
        Task<CompanyInvitation> AddAsync(CompanyInvitation invitation);
        Task<CompanyInvitation?> GetByTokenAsync(string token);
        Task<CompanyInvitation?> GetByIdAsync(int id);
        Task<IEnumerable<CompanyInvitation>> GetPendingByCompanyIdAsync(int companyId);
        Task UpdateAsync(CompanyInvitation invitation);
    }
}

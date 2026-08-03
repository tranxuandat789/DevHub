using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Services.Interfaces
{
    public interface ICompanyFollowerService
    {
        Task<bool> ToggleFollowAsync(int candidateId, int companyId);
        Task<bool> IsFollowingAsync(int candidateId, int companyId);
        Task<(List<Company> Items, int TotalCount)> GetFollowedCompaniesAsync(int candidateId, string? keyword, int page, int pageSize);
        Task<int> GetFollowedCompaniesCountAsync(int candidateId);
        Task<List<Candidate>> GetFollowersByCompanyAsync(int companyId);
    }
}

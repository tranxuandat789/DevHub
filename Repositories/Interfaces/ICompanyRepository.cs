using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<(List<Company> Items, int TotalCount)> GetVisibleCompaniesAsync(
            string? searchTerm, 
            List<int>? selectedTechs, 
            List<int>? selectedPositions, 
            string? sortOrder,
            int page, 
            int pageSize);
            
        Task<Company> AddCompanyAsync(Company company);
        Task<Company?> GetCompanyDetailsAsync(int companyId);
        Task<List<JobPost>> GetCompanyJobsAsync(int companyId);
        
        Task<List<CommonTechnology>> GetActiveTechnologiesAsync();
        Task<List<CommonJobPosition>> GetActiveJobPositionsAsync();
    }
}

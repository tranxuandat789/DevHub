using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<(List<Recruiter> Items, int TotalCount)> GetVisibleCompaniesAsync(
            string? searchTerm, 
            List<int>? selectedTechs, 
            List<int>? selectedPositions, 
            string? sortOrder,
            int page, 
            int pageSize);
            
        Task<Recruiter?> GetCompanyDetailsAsync(int recruiterId);
        Task<List<JobPost>> GetCompanyJobsAsync(int recruiterId);
        
        Task<List<CommonTechnology>> GetActiveTechnologiesAsync();
        Task<List<CommonJobPosition>> GetActiveJobPositionsAsync();
    }
}

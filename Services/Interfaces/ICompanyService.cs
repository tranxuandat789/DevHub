using System.Threading.Tasks;
using DevHub.ViewModels.Company;

namespace DevHub.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanySearchPageViewModel> SearchCompaniesAsync(CompanySearchInputViewModel input);
        Task<CompanyDetailsViewModel?> GetCompanyDetailsAsync(int id);
    }
}

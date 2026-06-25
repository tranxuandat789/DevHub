//03/06/2026 DatTX
using DevHub.ViewModels.Jobs;

namespace DevHub.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<JobRecommendationViewModel>> GetRecommendedJobsAsync(int candidateId);
    }
}

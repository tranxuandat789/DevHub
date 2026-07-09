//03/06/2026 DatTX
using DevHub.Models;

namespace DevHub.ViewModels.Jobs
{
    public class JobRecommendationViewModel
    {
        public JobPost Job { get; set; } = null!;
        public double MatchPercentage { get; set; }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IInterviewRepository
{
    Task<Application?> GetApplicationForInterviewAsync(int applicationId);
    Task<Interview> AddInterviewAsync(Interview interview);
    Task<Interview?> GetInterviewForRecruiterAsync(int interviewId, int recruiterId);
    Task UpdateAsync(Interview interview);
    Task<List<Interview>> GetPastScheduledInterviewsAsync();
    Task UpdateRangeAsync(IEnumerable<Interview> interviews);
}

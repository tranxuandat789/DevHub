using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICandidateSkillRepository
{
    Task<List<CandidateSkill>> GetByCandidateIdAsync(int candidateId);
    Task AddAsync(int candidateId, int techId, string? level);
    Task RemoveAsync(int candidateId, int techId);
}

using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface ICandidateSkillService
{
    Task<List<CandidateSkill>> GetSkillsAsync(int candidateId);
    Task AddSkillAsync(int candidateId, int techId, string? level);
    Task RemoveSkillAsync(int candidateId, int techId);
}

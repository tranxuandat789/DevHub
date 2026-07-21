//03/06/2026 DatTX
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class CandidateSkillService : ICandidateSkillService
{
    private readonly ICandidateSkillRepository _repo;
    public CandidateSkillService(ICandidateSkillRepository repo) => _repo = repo;

    public Task<List<CandidateSkill>> GetSkillsAsync(int candidateId)
        => _repo.GetByCandidateIdAsync(candidateId);

    public Task AddSkillAsync(int candidateId, int techId, string? level)
        => _repo.AddAsync(candidateId, techId, level);

    public Task RemoveSkillAsync(int candidateId, int techId)
        => _repo.RemoveAsync(candidateId, techId);
}

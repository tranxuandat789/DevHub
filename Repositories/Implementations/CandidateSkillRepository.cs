using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CandidateSkillRepository : ICandidateSkillRepository
{
    private readonly ItrecruitmentDbContext _db;
    public CandidateSkillRepository(ItrecruitmentDbContext db) => _db = db;

    public async Task<List<CandidateSkill>> GetByCandidateIdAsync(int candidateId)
    {
        return await _db.CandidateSkills
            .Include(cs => cs.Tech)
            .Where(cs => cs.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task AddAsync(int candidateId, int techId, string? level)
    {
        var exists = await _db.CandidateSkills
            .AnyAsync(cs => cs.CandidateId == candidateId && cs.TechId == techId);
        if (exists) return;

        _db.CandidateSkills.Add(new CandidateSkill
        {
            CandidateId = candidateId,
            TechId = techId,
            Level = level
        });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int candidateId, int techId)
    {
        await _db.CandidateSkills
            .Where(cs => cs.CandidateId == candidateId && cs.TechId == techId)
            .ExecuteDeleteAsync();
    }
}

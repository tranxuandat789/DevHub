using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CandidateRepository : ICandidateRepository
{
    private readonly ItrecruitmentDbContext _db;

    public CandidateRepository(ItrecruitmentDbContext db) => _db = db;

    public async Task<Candidate> AddAsync(Candidate candidate)
    {
        _db.Candidates.Add(candidate);
        await _db.SaveChangesAsync();
        return candidate;
    }

    public Task UpdateAvatarAsync(int candidateId, string avatarUrl)
        => _db.Candidates
              .Where(c => c.CandidateId == candidateId)
              .ExecuteUpdateAsync(s => s.SetProperty(c => c.ImageUrl, avatarUrl));
}

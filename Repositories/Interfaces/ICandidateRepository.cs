using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface ICandidateRepository
{
    Task<Candidate> AddAsync(Candidate candidate);
    Task UpdateAvatarAsync(int candidateId, string avatarUrl);
}

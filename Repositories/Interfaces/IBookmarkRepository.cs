using DevHub.Models;

namespace DevHub.Repositories.Interfaces;

public interface IBookmarkRepository
{
    Task<List<Bookmark>> GetByCandidateAsync(int candidateId);
    Task<bool> IsBookmarkedAsync(int candidateId, int jobId);
    Task AddAsync(int candidateId, int jobId);
    Task RemoveAsync(int candidateId, int jobId);
    Task<HashSet<int>> GetBookmarkedJobIdsAsync(int candidateId);
    Task<(List<Bookmark> Items, int TotalCount)> GetPagedAsync(
        int candidateId, int page, int pageSize,
        string? filterWorkingModel, string? filterLevel, string? filterLocation);
}

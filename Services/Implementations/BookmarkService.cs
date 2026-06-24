using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class BookmarkService : IBookmarkService
{
    private readonly IBookmarkRepository _repo;

    public BookmarkService(IBookmarkRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Bookmark>> GetByCandidateAsync(int candidateId)
        => _repo.GetByCandidateAsync(candidateId);

    public Task<bool> IsBookmarkedAsync(int candidateId, int jobId)
        => _repo.IsBookmarkedAsync(candidateId, jobId);

    public async Task<bool> ToggleAsync(int candidateId, int jobId)
    {
        var isBookmarked = await _repo.IsBookmarkedAsync(candidateId, jobId);
        if (isBookmarked)
        {
            await _repo.RemoveAsync(candidateId, jobId);
            return false;
        }
        await _repo.AddAsync(candidateId, jobId);
        return true;
    }

    public Task<HashSet<int>> GetBookmarkedJobIdsAsync(int candidateId)
        => _repo.GetBookmarkedJobIdsAsync(candidateId);

    public Task<(List<Bookmark> Items, int TotalCount)> GetPagedAsync(
        int candidateId, int page, int pageSize,
        string? filterWorkingModel, string? filterLevel, string? filterLocation)
        => _repo.GetPagedAsync(candidateId, page, pageSize, filterWorkingModel, filterLevel, filterLocation);
}

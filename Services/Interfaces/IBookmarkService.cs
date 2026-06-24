using DevHub.Models;

namespace DevHub.Services.Interfaces;

public interface IBookmarkService
{
    Task<List<Bookmark>> GetByCandidateAsync(int candidateId);
    Task<bool> IsBookmarkedAsync(int candidateId, int jobId);
    Task<bool> ToggleAsync(int candidateId, int jobId);
    Task<HashSet<int>> GetBookmarkedJobIdsAsync(int candidateId);
    Task<(List<Bookmark> Items, int TotalCount)> GetPagedAsync(
        int candidateId, int page, int pageSize,
        string? filterWorkingModel, string? filterLevel, string? filterLocation);
}

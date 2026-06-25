using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly ItrecruitmentDbContext _db;

    public BookmarkRepository(ItrecruitmentDbContext db)
    {
        _db = db;
    }

    public async Task<List<Bookmark>> GetByCandidateAsync(int candidateId)
    {
        return await _db.Bookmarks
            .Where(b => b.CandidateId == candidateId)
            .Include(b => b.Job)
                .ThenInclude(j => j.Recruiter)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsBookmarkedAsync(int candidateId, int jobId)
    {
        return await _db.Bookmarks
            .AnyAsync(b => b.CandidateId == candidateId && b.JobId == jobId);
    }

    public async Task AddAsync(int candidateId, int jobId)
    {
        var bookmark = new Bookmark
        {
            CandidateId = candidateId,
            JobId = jobId,
            CreatedAt = DateTime.Now
        };
        _db.Bookmarks.Add(bookmark);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int candidateId, int jobId)
    {
        var bookmark = await _db.Bookmarks
            .FirstOrDefaultAsync(b => b.CandidateId == candidateId && b.JobId == jobId);
        if (bookmark != null)
        {
            _db.Bookmarks.Remove(bookmark);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<HashSet<int>> GetBookmarkedJobIdsAsync(int candidateId)
    {
        var ids = await _db.Bookmarks
            .Where(b => b.CandidateId == candidateId)
            .Select(b => b.JobId)
            .ToListAsync();
        return ids.ToHashSet();
    }

    public async Task<(List<Bookmark> Items, int TotalCount)> GetPagedAsync(
        int candidateId, int page, int pageSize,
        string? filterWorkingModel, string? filterLevel, string? filterLocation)
    {
        var query = _db.Bookmarks
            .Where(b => b.CandidateId == candidateId)
            .Include(b => b.Job)
                .ThenInclude(j => j.Recruiter)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filterWorkingModel))
            query = query.Where(b => b.Job.WorkingModel == filterWorkingModel);

        if (!string.IsNullOrEmpty(filterLevel))
            query = query.Where(b => b.Job.ExperienceLevel == filterLevel);

        if (!string.IsNullOrEmpty(filterLocation))
            query = query.Where(b => b.Job.Location != null && b.Job.Location.Contains(filterLocation));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

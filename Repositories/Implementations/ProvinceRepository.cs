using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class ProvinceRepository : IProvinceRepository
{
    private readonly ItrecruitmentDbContext _context;

    public ProvinceRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Province>> GetAllAsync()
        => await _context.Provinces
            .AsNoTracking()
            .OrderBy(p => p.ProvinceName)
            .ToListAsync();

    public async Task<List<Province>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new List<Province>();

        return await _context.Provinces
            .Where(p => idList.Contains(p.ProvinceId))
            .ToListAsync();
    }
}

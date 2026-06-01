using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CommonTechnologyRepository : ICommonTechnologyRepository
{
    private readonly ItrecruitmentDbContext _context;
    public CommonTechnologyRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }
    public async Task<List<CommonTechnology>> GetByIdsAsync(List<int> ids)
    {
        if (ids == null || ids.Count == 0) return new List<CommonTechnology>();
        return await _context.CommonTechnologies.Where(t => ids.Contains(t.TechId) && (t.IsActive == true || t.IsActive == null)).ToListAsync();
    }
    public async Task<List<CommonTechnology>> GetAllActiveAsync()
    {
        return await _context.CommonTechnologies.Where(t => t.IsActive == true || t.IsActive == null).ToListAsync();
    }

}

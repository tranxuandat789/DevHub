using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class CommonJobPositionRepository : ICommonJobPositionRepository
{
    private readonly ItrecruitmentDbContext _context;
    public CommonJobPositionRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<CommonJobPosition?> GetByIdAsync(int positionId)
    {
        return await _context.CommonJobPositions.FirstOrDefaultAsync(p => p.PositionId == positionId && (p.IsActive == true || p.IsActive == null));
    }

    public async Task<List<CommonJobPosition>> GetAllActiveAsync()
    {
        return await _context.CommonJobPositions.Where(p => p.IsActive == true || p.IsActive == null).ToListAsync();
    }
}

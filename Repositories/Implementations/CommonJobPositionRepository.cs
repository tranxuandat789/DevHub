//AnhPT-01/06/2026
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
        return await _context.CommonJobPositions.FirstOrDefaultAsync(p => p.PositionId == positionId);
    }

    public async Task<List<CommonJobPosition>> GetAllActiveAsync()
    {
        return await _context.CommonJobPositions.Where(p => p.IsActive == true || p.IsActive == null).ToListAsync();
    }

    public async Task<List<CommonJobPosition>> GetAllAsync()
    {
        return await _context.CommonJobPositions.ToListAsync();
    }

    public async Task UpdateAsync(CommonJobPosition position)
    {
        _context.CommonJobPositions.Update(position);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(CommonJobPosition position)
    {
        await _context.CommonJobPositions.AddAsync(position);
        await _context.SaveChangesAsync();
    }
}

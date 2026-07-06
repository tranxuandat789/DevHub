using DevHub.Models;
using DevHub.Data;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Repositories.Implementations;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ItrecruitmentDbContext _context;

    public AuditLogRepository(ItrecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }
}

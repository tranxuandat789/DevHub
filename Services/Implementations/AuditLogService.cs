using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

namespace DevHub.Services.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync()
    {
        return await _auditLogRepository.GetLogsAsync();
    }
}

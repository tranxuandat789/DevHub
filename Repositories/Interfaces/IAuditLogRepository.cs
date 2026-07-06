namespace DevHub.Repositories.Interfaces;
using DevHub.Models;

public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLog>> GetLogsAsync();
}

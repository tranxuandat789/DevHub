namespace DevHub.Services.Interfaces;
using DevHub.Models;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLog>> GetLogsAsync();
}

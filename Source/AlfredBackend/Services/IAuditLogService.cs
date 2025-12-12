using AlfredBackend.Models;

namespace AlfredBackend.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(AuditLogDocument log);
        Task<List<AuditLogDocument>> GetUserLogsAsync(string userId, DateTime? from = null, DateTime? to = null, int limit = 1000);
        Task<List<AuditLogDocument>> GetLogsByActionAsync(string action, int limit = 100);
    }
}

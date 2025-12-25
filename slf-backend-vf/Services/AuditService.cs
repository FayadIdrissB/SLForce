using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface IAuditService
{
    Task LogAsync(int? userId, string action, string description);
}

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(AppDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(int? userId, string action, string description)
    {
        try
        {
            var auditLog = new AuditLog
            {
                IdUser = userId,
                Action = action,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'enregistrement de l'audit log pour l'utilisateur {UserId}, action: {Action}", userId, action);
        }
    }
}


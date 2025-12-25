using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.DTO.Moderation;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface IModerationService
{
    Task<bool> BlockUserAsync(int blockerId, BlockUserDto dto);
    Task<bool> ReportUserAsync(int reporterId, ReportUserDto dto);
}

public class ModerationService : IModerationService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<ModerationService> _logger;

    public ModerationService(
        AppDbContext context,
        IAuditService auditService,
        ILogger<ModerationService> logger)
    {
        _context = context;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<bool> BlockUserAsync(int blockerId, BlockUserDto dto)
    {
        if (blockerId == dto.UserIdToBlock)
        {
            throw new Exception("Un utilisateur ne peut pas se bloquer lui-même");
        }

        var blocker = await _context.Users.FindAsync(blockerId);
        var blocked = await _context.Users.FindAsync(dto.UserIdToBlock);

        if (blocker == null || blocked == null)
        {
            throw new Exception("Utilisateur non trouvé");
        }

        var existingBlock = await _context.UserBlocks
            .FirstOrDefaultAsync(ub => ub.IdUserBlocker == blockerId && ub.IdUserBlocked == dto.UserIdToBlock);

        if (existingBlock != null)
        {
            existingBlock.Status = true;
            existingBlock.Reason = dto.Reason;
        }
        else
        {
            var userBlock = new UserBlock
            {
                IdUserBlocker = blockerId,
                IdUserBlocked = dto.UserIdToBlock,
                Status = true,
                Reason = dto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserBlocks.Add(userBlock);
        }

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(blockerId, "BLOCK_USER", $"Blocage de l'utilisateur {dto.UserIdToBlock}");

        _logger.LogInformation("Utilisateur {BlockedId} bloqué par {BlockerId}", dto.UserIdToBlock, blockerId);

        return true;
    }

    public async Task<bool> ReportUserAsync(int reporterId, ReportUserDto dto)
    {
        if (reporterId == dto.UserIdToReport)
        {
            throw new Exception("Un utilisateur ne peut pas se signaler lui-même");
        }

        var reporter = await _context.Users.FindAsync(reporterId);
        var reported = await _context.Users.FindAsync(dto.UserIdToReport);

        if (reporter == null || reported == null)
        {
            throw new Exception("Utilisateur non trouvé");
        }

        var userReport = new UserReport
        {
            IdUserReporter = reporterId,
            IdUserReported = dto.UserIdToReport,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.UserReports.Add(userReport);
        await _context.SaveChangesAsync();

        var report = new Report
        {
            IdUser = reporterId,
            IdReport = userReport.IdReport
        };
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(reporterId, "REPORT_USER", $"Signalement de l'utilisateur {dto.UserIdToReport}");

        _logger.LogInformation("Utilisateur {ReportedId} signalé par {ReporterId}", dto.UserIdToReport, reporterId);

        return true;
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.Entities;

namespace slf_backend.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "IsAdmin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(List<AuditLogResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int? userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(al => al.IdUser == userId.Value);
        }

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(al => al.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = logs.Select(log => new AuditLogResponseDto
        {
            Id = log.IdLog,
            UserId = log.IdUser,
            Action = log.Action,
            Description = log.Description,
            CreatedAt = log.CreatedAt
        }).ToList();

        return Ok(new
        {
            data = result,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    [HttpGet("reports")]
    [ProducesResponseType(typeof(List<ReportResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _context.UserReports
            .Include(ur => ur.Reporter)
            .Include(ur => ur.Reported)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(ur => ur.Status == status);
        }

        var totalCount = await query.CountAsync();
        var reports = await query
            .OrderByDescending(ur => ur.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = reports.Select(report => new ReportResponseDto
        {
            Id = report.IdReport,
            ReporterId = report.IdUserReporter,
            ReporterName = $"{report.Reporter.FirstName} {report.Reporter.LastName}",
            ReportedId = report.IdUserReported,
            ReportedName = $"{report.Reported.FirstName} {report.Reported.LastName}",
            Reason = report.Reason,
            Status = report.Status,
            CreatedAt = report.CreatedAt
        }).ToList();

        return Ok(new
        {
            data = result,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
}

public class AuditLogResponseDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ReportResponseDto
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public int ReportedId { get; set; }
    public string ReportedName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slf_backend.DTO.Moderation;
using slf_backend.Services;
using System.Security.Claims;

namespace slf_backend.Controllers;

[ApiController]
[Route("api/v1/moderation")]
[Authorize]
public class ModerationController : ControllerBase
{
    private readonly IModerationService _moderationService;
    private readonly ILogger<ModerationController> _logger;

    public ModerationController(IModerationService moderationService, ILogger<ModerationController> logger)
    {
        _moderationService = moderationService;
        _logger = logger;
    }

    [HttpPost("block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var blockerId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _moderationService.BlockUserAsync(blockerId, dto);
            return Ok(new { message = "Utilisateur bloqué avec succès", result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du blocage de l'utilisateur");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReportUser([FromBody] ReportUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var reporterId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _moderationService.ReportUserAsync(reporterId, dto);
            return Ok(new { message = "Utilisateur signalé avec succès", result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du signalement de l'utilisateur");
            return BadRequest(new { message = ex.Message });
        }
    }
}


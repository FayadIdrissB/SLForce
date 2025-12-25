using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.DTO.Links;
using slf_backend.Services;
using System.Security.Claims;

namespace slf_backend.Controllers;

[ApiController]
[Route("api/v1/links")]
[Authorize]
public class LinksController : ControllerBase
{
    private readonly ILinkService _linkService;
    private readonly AppDbContext _context;
    private readonly ILogger<LinksController> _logger;

    public LinksController(ILinkService linkService, AppDbContext context, ILogger<LinksController> logger)
    {
        _linkService = linkService;
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "IsCoach")]
    [ProducesResponseType(typeof(LinkResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLink([FromBody] CreateLinkDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var coachId))
        {
            return Unauthorized();
        }

        try
        {
            var link = await _linkService.CreateLinkAsync(coachId, dto);
            return CreatedAtAction(nameof(CreateLink), link);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du lien");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-athletes")]
    [Authorize(Policy = "IsCoach")]
    [ProducesResponseType(typeof(List<LinkResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAthletes()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var coachId))
        {
            return Unauthorized();
        }

        try
        {
            var athletes = await _linkService.GetMyAthletesAsync(coachId);
            return Ok(athletes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des athlètes");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-coach")]
    [Authorize(Policy = "IsAthlete")]
    [ProducesResponseType(typeof(LinkResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyCoach()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var athleteId))
        {
            return Unauthorized();
        }

        try
        {
            var coach = await _linkService.GetMyCoachAsync(athleteId);
            if (coach == null)
            {
                return NotFound(new { message = "Aucun coach trouvé" });
            }
            return Ok(coach);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du coach");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{athleteId}/cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelLink(int athleteId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            int coachEntityId;

            if (role == "Coach")
            {
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.UserId == userId);
                if (coach == null)
                {
                    return NotFound(new { message = "Coach non trouvé" });
                }
                coachEntityId = coach.Id;
            }
            else if (role == "Athlete")
            {
                var athlete = await _context.Athletes
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == athleteId);
                if (athlete == null)
                {
                    return Forbid();
                }
                var link = await _context.Links
                    .FirstOrDefaultAsync(l => l.IdUserAthlete == athleteId);
                if (link == null)
                {
                    return NotFound(new { message = "Lien non trouvé" });
                }
                coachEntityId = link.IdUserCoach;
            }
            else
            {
                return Forbid();
            }

            await _linkService.CancelLinkAsync(athleteId, coachEntityId);
            return Ok(new { message = "Lien annulé avec succès" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'annulation du lien");
            return BadRequest(new { message = ex.Message });
        }
    }
}


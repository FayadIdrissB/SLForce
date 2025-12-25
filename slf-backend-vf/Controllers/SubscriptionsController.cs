using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slf_backend.DTO.Subscriptions;
using slf_backend.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace slf_backend.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
[Authorize]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    [HttpPost("app/start")]
    [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartAppSubscription([FromBody] StartSubscriptionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var subscription = await _subscriptionService.StartAppSubscriptionAsync(userId, dto.Price);
            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du démarrage de l'abonnement app");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("app/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelAppSubscription()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var cancelled = await _subscriptionService.CancelAppSubscriptionAsync(userId);
            return Ok(new { message = "Abonnement app annulé", cancelled });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'annulation de l'abonnement app");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("coach/start")]
    [Authorize(Policy = "IsCoach")]
    [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartCoachSubscription([FromBody] StartSubscriptionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var subscription = await _subscriptionService.StartCoachSubscriptionAsync(userId, dto.Price);
            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du démarrage de l'abonnement coach");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("coach/cancel")]
    [Authorize(Policy = "IsCoach")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelCoachSubscription()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var cancelled = await _subscriptionService.CancelCoachSubscriptionAsync(userId);
            return Ok(new { message = "Abonnement coach annulé", cancelled });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'annulation de l'abonnement coach");
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class StartSubscriptionDto
{
    [Required(ErrorMessage = "Le prix est requis")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
    public decimal Price { get; set; }
}


using Microsoft.AspNetCore.Mvc;
using slf_backend.Services;
using Stripe;

namespace slf_backend.Controllers;

[ApiController]
[Route("api/v1/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly ILogger<WebhooksController> _logger;
    private readonly IConfiguration _configuration;

    public WebhooksController(
        IStripeService stripeService,
        ILogger<WebhooksController> logger,
        IConfiguration configuration)
    {
        _stripeService = stripeService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("stripe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                webhookSecret
            );

            _logger.LogInformation("Webhook Stripe reçu: {EventType}, {EventId}", stripeEvent.Type, stripeEvent.Id);

            switch (stripeEvent.Type)
            {
                case Events.CustomerSubscriptionCreated:
                    await HandleSubscriptionCreated(stripeEvent);
                    break;

                case Events.CustomerSubscriptionUpdated:
                    await HandleSubscriptionUpdated(stripeEvent);
                    break;

                case Events.CustomerSubscriptionDeleted:
                    await HandleSubscriptionDeleted(stripeEvent);
                    break;

                default:
                    _logger.LogInformation("Type d'événement Stripe non géré: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erreur lors du traitement du webhook Stripe");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors du traitement du webhook Stripe");
            return BadRequest(new { message = ex.Message });
        }
    }

    private async Task HandleSubscriptionCreated(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        if (subscription == null)
        {
            _logger.LogWarning("Subscription null dans l'événement {EventId}", stripeEvent.Id);
            return;
        }

        _logger.LogInformation("Abonnement créé: {SubscriptionId}, Status: {Status}", subscription.Id, subscription.Status);
    }

    private async Task HandleSubscriptionUpdated(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        if (subscription == null)
        {
            _logger.LogWarning("Subscription null dans l'événement {EventId}", stripeEvent.Id);
            return;
        }

        var updated = await _stripeService.UpdateSubscriptionStatusAsync(
            subscription.Id,
            subscription.Status,
            "app"
        );

        if (!updated)
        {
            await _stripeService.UpdateSubscriptionStatusAsync(
                subscription.Id,
                subscription.Status,
                "coach"
            );
        }

        _logger.LogInformation("Abonnement mis à jour: {SubscriptionId}, Nouveau status: {Status}", subscription.Id, subscription.Status);
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        if (subscription == null)
        {
            _logger.LogWarning("Subscription null dans l'événement {EventId}", stripeEvent.Id);
            return;
        }

        var cancelled = await _stripeService.CancelSubscriptionAsync(subscription.Id, "app");
        if (!cancelled)
        {
            await _stripeService.CancelSubscriptionAsync(subscription.Id, "coach");
        }

        _logger.LogInformation("Abonnement supprimé: {SubscriptionId}", subscription.Id);
    }
}


using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.DTO.Subscriptions;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface ISubscriptionService
{
    Task<SubscriptionResponseDto> StartAppSubscriptionAsync(int userId, decimal price);
    Task<bool> CancelAppSubscriptionAsync(int userId);
    Task<SubscriptionResponseDto> StartCoachSubscriptionAsync(int userId, decimal price);
    Task<bool> CancelCoachSubscriptionAsync(int userId);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IStripeService _stripeService;
    private readonly IAuditService _auditService;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        AppDbContext context,
        IStripeService stripeService,
        IAuditService auditService,
        ILogger<SubscriptionService> logger)
    {
        _context = context;
        _stripeService = stripeService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<SubscriptionResponseDto> StartAppSubscriptionAsync(int userId, decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException("Le prix doit être supérieur à 0");
        }

        var user = await _context.Users
            .Include(u => u.SubscriptionAppStripe)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("Utilisateur non trouvé");
        }

        if (user.SubscriptionAppStripe.StatusSubscription == "active")
        {
            await _stripeService.CancelSubscriptionAsync(user.SubscriptionAppStripe.IdStripe, "app");
        }

        var subscription = await _stripeService.CreateAppSubscriptionAsync(price, userId);

        user.SubscriptionAppStripeId = subscription.Id;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(userId, "SUBSCRIPTION_APP_START", $"Démarrage abonnement app: {price}€");

        return new SubscriptionResponseDto
        {
            Id = subscription.Id,
            StripeId = subscription.IdStripe,
            StartSubscription = subscription.StartSubscription,
            EndSubscription = subscription.EndSubscription,
            Status = subscription.StatusSubscription,
            Price = subscription.Price,
            CreatedAt = subscription.CreatedAt
        };
    }

    public async Task<bool> CancelAppSubscriptionAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.SubscriptionAppStripe)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("Utilisateur non trouvé");
        }

        var cancelled = await _stripeService.CancelSubscriptionAsync(user.SubscriptionAppStripe.IdStripe, "app");

        await _auditService.LogAsync(userId, "SUBSCRIPTION_APP_CANCEL", "Annulation abonnement app");

        return cancelled;
    }

    public async Task<SubscriptionResponseDto> StartCoachSubscriptionAsync(int userId, decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException("Le prix doit être supérieur à 0");
        }

        var user = await _context.Users
            .Include(u => u.CoachProfile)
            .ThenInclude(c => c.CoachSubscriptionStripe)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("Utilisateur non trouvé");
        }

        if (user.CoachProfile == null)
        {
            throw new Exception("L'utilisateur n'est pas un coach");
        }

        if (user.CoachProfile.CoachSubscriptionStripe.StatusSubscription == "active")
        {
            await _stripeService.CancelSubscriptionAsync(user.CoachProfile.CoachSubscriptionStripe.IdStripe, "coach");
        }

        var subscription = await _stripeService.CreateCoachSubscriptionAsync(price, userId);

        user.CoachProfile.CoachSubscriptionStripeId = subscription.Id;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(userId, "SUBSCRIPTION_COACH_START", $"Démarrage abonnement coach: {price}€");

        return new SubscriptionResponseDto
        {
            Id = subscription.Id,
            StripeId = subscription.IdStripe,
            StartSubscription = subscription.StartSubscription,
            EndSubscription = subscription.EndSubscription,
            Status = subscription.StatusSubscription,
            Price = subscription.Price,
            CreatedAt = subscription.CreatedAt
        };
    }

    public async Task<bool> CancelCoachSubscriptionAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.CoachProfile)
            .ThenInclude(c => c.CoachSubscriptionStripe)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception("Utilisateur non trouvé");
        }

        if (user.CoachProfile == null)
        {
            throw new Exception("L'utilisateur n'est pas un coach");
        }

        var cancelled = await _stripeService.CancelSubscriptionAsync(user.CoachProfile.CoachSubscriptionStripe.IdStripe, "coach");

        await _auditService.LogAsync(userId, "SUBSCRIPTION_COACH_CANCEL", "Annulation abonnement coach");

        return cancelled;
    }
}


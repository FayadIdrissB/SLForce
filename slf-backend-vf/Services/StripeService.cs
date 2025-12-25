using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.Entities;
using Stripe;

namespace slf_backend.Services;

public interface IStripeService
{
    Task<SubscriptionAppStripe> CreateAppSubscriptionAsync(decimal price, int userId);
    Task<SubscriptionCoachStripe> CreateCoachSubscriptionAsync(decimal price, int userId);
    Task<bool> CancelSubscriptionAsync(string stripeSubscriptionId, string subscriptionType);
    Task<bool> UpdateSubscriptionStatusAsync(string stripeSubscriptionId, string status, string subscriptionType);
}

public class StripeService : IStripeService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripeService> _logger;

    public StripeService(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<StripeService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<SubscriptionAppStripe> CreateAppSubscriptionAsync(decimal price, int userId)
    {
        try
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = await GetOrCreateCustomerAsync(userId),
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        PriceData = new SubscriptionItemPriceDataOptions
                        {
                            UnitAmount = (long)(price * 100),
                            Currency = "eur",
                            Recurring = new SubscriptionItemPriceDataRecurringOptions
                            {
                                Interval = "month"
                            },
                            ProductData = new SubscriptionItemPriceDataProductDataOptions
                            {
                                Name = "Abonnement App SLForce"
                            }
                        }
                    }
                }
            };

            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(options);

            var appSubscription = new SubscriptionAppStripe
            {
                IdStripe = subscription.Id,
                StartSubscription = subscription.CurrentPeriodStart,
                EndSubscription = subscription.CurrentPeriodEnd,
                StatusSubscription = subscription.Status,
                Price = price,
                CreatedAt = DateTime.UtcNow
            };

            _context.SubscriptionAppStripes.Add(appSubscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Abonnement app Stripe créé: {StripeId} pour l'utilisateur {UserId}", subscription.Id, userId);

            return appSubscription;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erreur Stripe lors de la création de l'abonnement app pour l'utilisateur {UserId}", userId);
            throw;
        }
    }

    public async Task<SubscriptionCoachStripe> CreateCoachSubscriptionAsync(decimal price, int userId)
    {
        try
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = await GetOrCreateCustomerAsync(userId),
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        PriceData = new SubscriptionItemPriceDataOptions
                        {
                            UnitAmount = (long)(price * 100),
                            Currency = "eur",
                            Recurring = new SubscriptionItemPriceDataRecurringOptions
                            {
                                Interval = "month"
                            },
                            ProductData = new SubscriptionItemPriceDataProductDataOptions
                            {
                                Name = "Abonnement Coach SLForce"
                            }
                        }
                    }
                }
            };

            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(options);

            var coachSubscription = new SubscriptionCoachStripe
            {
                IdStripe = subscription.Id,
                StartSubscription = subscription.CurrentPeriodStart,
                EndSubscription = subscription.CurrentPeriodEnd,
                StatusSubscription = subscription.Status,
                Price = price,
                CreatedAt = DateTime.UtcNow
            };

            _context.SubscriptionCoachStripes.Add(coachSubscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Abonnement coach Stripe créé: {StripeId} pour l'utilisateur {UserId}", subscription.Id, userId);

            return coachSubscription;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erreur Stripe lors de la création de l'abonnement coach pour l'utilisateur {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> CancelSubscriptionAsync(string stripeSubscriptionId, string subscriptionType)
    {
        try
        {
            var service = new SubscriptionService();
            var subscription = await service.CancelAsync(stripeSubscriptionId);

            if (subscriptionType == "app")
            {
                var appSub = await _context.SubscriptionAppStripes
                    .FirstOrDefaultAsync(s => s.IdStripe == stripeSubscriptionId);
                
                if (appSub != null)
                {
                    appSub.StatusSubscription = subscription.Status;
                    await _context.SaveChangesAsync();
                }
            }
            else if (subscriptionType == "coach")
            {
                var coachSub = await _context.SubscriptionCoachStripes
                    .FirstOrDefaultAsync(s => s.IdStripe == stripeSubscriptionId);
                
                if (coachSub != null)
                {
                    coachSub.StatusSubscription = subscription.Status;
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Abonnement Stripe annulé: {StripeId}", stripeSubscriptionId);
            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erreur Stripe lors de l'annulation de l'abonnement {StripeId}", stripeSubscriptionId);
            return false;
        }
    }

    public async Task<bool> UpdateSubscriptionStatusAsync(string stripeSubscriptionId, string status, string subscriptionType)
    {
        try
        {
            if (subscriptionType == "app")
            {
                var appSub = await _context.SubscriptionAppStripes
                    .FirstOrDefaultAsync(s => s.IdStripe == stripeSubscriptionId);
                
                if (appSub != null)
                {
                    appSub.StatusSubscription = status;
                    await _context.SaveChangesAsync();
                }
            }
            else if (subscriptionType == "coach")
            {
                var coachSub = await _context.SubscriptionCoachStripes
                    .FirstOrDefaultAsync(s => s.IdStripe == stripeSubscriptionId);
                
                if (coachSub != null)
                {
                    coachSub.StatusSubscription = status;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du statut de l'abonnement {StripeId}", stripeSubscriptionId);
            return false;
        }
    }

    private async Task<string> GetOrCreateCustomerAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception($"Utilisateur {userId} non trouvé");
        }

        var customerOptions = new CustomerCreateOptions
        {
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}"
        };

        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(customerOptions);

        return customer.Id;
    }
}


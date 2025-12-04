namespace slf_backend.Entities;

public class SubscriptionAppStripe
{
    public int Id { get; set; }
    public string StripeId { get; set; } = default!;
    public DateTime StartSubscription { get; set; }
    public DateTime EndSubscription { get; set; }
    public string StatusSubscription { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}

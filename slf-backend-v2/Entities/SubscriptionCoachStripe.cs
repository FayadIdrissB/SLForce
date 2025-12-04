namespace slf_backend.Entities;

public class SubscriptionCoachStripe
{
    public int Id { get; set; }
    public string StripeId { get; set; } = default!;
    public DateTime StartSubscription { get; set; }
    public DateTime EndSubscription { get; set; }
    public string StatusSubscription { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();
    public ICollection<Coach> Coaches { get; set; } = new List<Coach>();
}

namespace slf_backend.Entities;

public class SubscriptionCoachStripe
{
    public int Id { get; set; }
    public string IdStripe { get; set; } = string.Empty;
    public DateTime StartSubscription { get; set; }
    public DateTime EndSubscription { get; set; }
    public string StatusSubscription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();
    public ICollection<Coach> Coaches { get; set; } = new List<Coach>();
}


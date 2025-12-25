namespace slf_backend.Entities;

public class SubscriptionAppStripe
{
    public int Id { get; set; }
    public string IdStripe { get; set; } = string.Empty;
    public DateTime StartSubscription { get; set; }
    public DateTime EndSubscription { get; set; }
    public string StatusSubscription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}


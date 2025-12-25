namespace slf_backend.Entities;

public class Athlete
{
    public int Id { get; set; }
    public string WeightCategory { get; set; } = string.Empty;
    
    // Foreign Keys
    public int CoachSubscriptionStripeId { get; set; }
    public SubscriptionCoachStripe CoachSubscriptionStripe { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Navigation - Links
    public ICollection<Link> Links { get; set; } = new List<Link>();
}


namespace slf_backend.Entities;

public class Coach
{
    public int Id { get; set; }
    public decimal MonthPrice { get; set; }
    public string Biography { get; set; } = string.Empty;
    public string Specialities { get; set; } = string.Empty;
    public int CompletedSessions { get; set; }
    public decimal Rating { get; set; }
    
    // Foreign Keys
    public int CoachSubscriptionStripeId { get; set; }
    public SubscriptionCoachStripe CoachSubscriptionStripe { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Navigation - Links
    public ICollection<Link> Links { get; set; } = new List<Link>();
}


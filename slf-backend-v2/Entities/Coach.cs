namespace slf_backend.Entities;

public class Coach
{
    public int Id { get; set; }
    public decimal MonthPrice { get; set; }
    public string Biography { get; set; } = default!;
    public string Specialities { get; set; } = default!;
    public int CompletedSessions { get; set; }
    public decimal Rating { get; set; }

    public int CoachSubscriptionStripeId { get; set; }
    public SubscriptionCoachStripe CoachSubscriptionStripe { get; set; } = default!;

    public int UserId { get; set; }
    public User User { get; set; } = default!;
}

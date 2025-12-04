namespace slf_backend.Entities;

public class Athlete
{
    public int Id { get; set; }
    public string WeightCategory { get; set; } = default!;

    public int CoachSubscriptionStripeId { get; set; }
    public SubscriptionCoachStripe CoachSubscriptionStripe { get; set; } = default!;

    public int UserId { get; set; }
    public User User { get; set; } = default!;
}

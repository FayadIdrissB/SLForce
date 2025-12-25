namespace slf_backend.DTO.Subscriptions;

public class SubscriptionResponseDto
{
    public int Id { get; set; }
    public string StripeId { get; set; } = string.Empty;
    public DateTime StartSubscription { get; set; }
    public DateTime EndSubscription { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}


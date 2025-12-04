namespace slf_backend.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;
}

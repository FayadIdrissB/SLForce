namespace slf_backend.Entities;

public class Session
{
    public int Id { get; set; }
    public string DeviceType { get; set; } = default!;
    public string IpAddress { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;
}

namespace slf_backend.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;
}

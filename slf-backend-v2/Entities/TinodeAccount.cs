namespace slf_backend.Entities;

public class TinodeAccount
{
    public int Id { get; set; }
    public string TinodeUserId { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;
}

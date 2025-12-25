namespace slf_backend.Entities;

public class UserBlock
{
    public int IdBlock { get; set; }
    public int IdUserBlocker { get; set; }
    public int IdUserBlocked { get; set; }
    public bool? Status { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public User Blocker { get; set; } = null!;
    public User Blocked { get; set; } = null!;
    
    // Table de jointure
    public ICollection<Blocage> Blocages { get; set; } = new List<Blocage>();
}


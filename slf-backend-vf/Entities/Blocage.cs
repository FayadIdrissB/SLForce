namespace slf_backend.Entities;

// Table de jointure User_ <-> user_block
public class Blocage
{
    public int IdUser { get; set; }
    public int IdBlock { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public UserBlock UserBlock { get; set; } = null!;
}


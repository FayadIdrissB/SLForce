namespace slf_backend.Entities;

public class TinodeAccount
{
    public int IdTinodeAccount { get; set; }
    public string TinodeUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public int? IdUser { get; set; }
    public User? User { get; set; }
}


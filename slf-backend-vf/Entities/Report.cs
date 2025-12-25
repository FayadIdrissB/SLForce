namespace slf_backend.Entities;

// Table de jointure User_ <-> user_report
public class Report
{
    public int IdUser { get; set; }
    public int IdReport { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public UserReport UserReport { get; set; } = null!;
}


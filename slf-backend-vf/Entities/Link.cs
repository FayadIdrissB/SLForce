namespace slf_backend.Entities;

public class Link
{
    // Clé composite
    public int IdUserAthlete { get; set; }
    public int IdUserCoach { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    
    // Navigation
    public Athlete Athlete { get; set; } = null!;
    public Coach Coach { get; set; } = null!;
}


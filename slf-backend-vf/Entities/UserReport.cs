namespace slf_backend.Entities;

public class UserReport
{
    public int IdReport { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int IdUserReporter { get; set; }
    public int IdUserReported { get; set; }
    
    // Navigation
    public User Reporter { get; set; } = null!;
    public User Reported { get; set; } = null!;
    
    // Table de jointure
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}


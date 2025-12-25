namespace slf_backend.Entities;

public class AuditLog
{
    public int IdLog { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public int? IdUser { get; set; }
    public User? User { get; set; }
}


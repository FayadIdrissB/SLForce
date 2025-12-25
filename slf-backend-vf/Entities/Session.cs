namespace slf_backend.Entities;

public class Session
{
    public int IdSession { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string IpAdress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public int? IdUser { get; set; }
    public User? User { get; set; }
}


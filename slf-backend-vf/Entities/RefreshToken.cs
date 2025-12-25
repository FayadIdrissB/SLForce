namespace slf_backend.Entities;

public class RefreshToken
{
    public int IdRefresh { get; set; }
    public string RefreshTokenValue { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    
    public int? IdUser { get; set; }
    public User? User { get; set; }
}


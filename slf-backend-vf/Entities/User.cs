namespace slf_backend.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Hash stocké
    public bool RoleAdmin { get; set; }
    
    // Foreign Key
    public int SubscriptionAppStripeId { get; set; }
    public SubscriptionAppStripe SubscriptionAppStripe { get; set; } = null!;
    
    // Navigation properties
    public Athlete? AthleteProfile { get; set; }
    public Coach? CoachProfile { get; set; }
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public TinodeAccount? TinodeAccount { get; set; }
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    
    // Relations réflexives - Blocks
    public ICollection<UserBlock> UserBlocksMade { get; set; } = new List<UserBlock>();
    public ICollection<UserBlock> UserBlocksReceived { get; set; } = new List<UserBlock>();
    
    // Relations réflexives - Reports
    public ICollection<UserReport> UserReportsMade { get; set; } = new List<UserReport>();
    public ICollection<UserReport> UserReportsReceived { get; set; } = new List<UserReport>();
    
    // Tables de jointure
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Blocage> Blocages { get; set; } = new List<Blocage>();
}


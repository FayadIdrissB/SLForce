using Microsoft.EntityFrameworkCore;
using slf_backend.Entities;

namespace slf_backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // =====================
    // DbSets
    // =====================
    public DbSet<User> Users => Set<User>();
    public DbSet<SubscriptionAppStripe> SubscriptionAppStripes => Set<SubscriptionAppStripe>();
    public DbSet<SubscriptionCoachStripe> SubscriptionCoachStripes => Set<SubscriptionCoachStripe>();
    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<Link> Links => Set<Link>();
    public DbSet<UserBlock> UserBlocks => Set<UserBlock>();
    public DbSet<UserReport> UserReports => Set<UserReport>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TinodeAccount> TinodeAccounts => Set<TinodeAccount>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Blocage> Blocages => Set<Blocage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ======================================
        //  DECIMAL PRECISION (SQL Server)
        // ======================================
        modelBuilder.Entity<SubscriptionAppStripe>()
            .Property(s => s.Price)
            .HasPrecision(15, 2);

        modelBuilder.Entity<SubscriptionCoachStripe>()
            .Property(s => s.Price)
            .HasPrecision(15, 2);

        modelBuilder.Entity<Coach>()
            .Property(c => c.MonthPrice)
            .HasPrecision(15, 2);

        modelBuilder.Entity<Coach>()
            .Property(c => c.Rating)
            .HasPrecision(3, 2);

        // ======================================
        //  User -> SubscriptionAppStripe
        // ======================================
        modelBuilder.Entity<User>()
            .HasOne(u => u.SubscriptionAppStripe)
            .WithMany(s => s.Users)
            .HasForeignKey(u => u.SubscriptionAppStripeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  User -> Athlete (1:1)
        // ======================================
        modelBuilder.Entity<Athlete>()
            .HasOne(a => a.User)
            .WithOne(u => u.AthleteProfile)
            .HasForeignKey<Athlete>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  User -> Coach (1:1)
        // ======================================
        modelBuilder.Entity<Coach>()
            .HasOne(c => c.User)
            .WithOne(u => u.CoachProfile)
            .HasForeignKey<Coach>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  Athlete -> SubscriptionCoachStripe
        // ======================================
        modelBuilder.Entity<Athlete>()
            .HasOne(a => a.CoachSubscriptionStripe)
            .WithMany(s => s.Athletes)
            .HasForeignKey(a => a.CoachSubscriptionStripeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  Coach -> SubscriptionCoachStripe
        // ======================================
        modelBuilder.Entity<Coach>()
            .HasOne(c => c.CoachSubscriptionStripe)
            .WithMany(s => s.Coaches)
            .HasForeignKey(c => c.CoachSubscriptionStripeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  Link (Clé composite: Athlete ↔ Coach)
        // ======================================
        modelBuilder.Entity<Link>()
            .HasKey(l => new { l.IdUserAthlete, l.IdUserCoach });

        modelBuilder.Entity<Link>()
            .HasOne(l => l.Athlete)
            .WithMany(a => a.Links)
            .HasForeignKey(l => l.IdUserAthlete)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Link>()
            .HasOne(l => l.Coach)
            .WithMany(c => c.Links)
            .HasForeignKey(l => l.IdUserCoach)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  UserBlock (Relations réflexives)
        // ======================================
        modelBuilder.Entity<UserBlock>()
            .HasKey(ub => ub.IdBlock);

        modelBuilder.Entity<UserBlock>()
            .HasOne(ub => ub.Blocker)
            .WithMany(u => u.UserBlocksMade)
            .HasForeignKey(ub => ub.IdUserBlocker)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserBlock>()
            .HasOne(ub => ub.Blocked)
            .WithMany(u => u.UserBlocksReceived)
            .HasForeignKey(ub => ub.IdUserBlocked)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  UserReport (Relations réflexives)
        // ======================================
        modelBuilder.Entity<UserReport>()
            .HasKey(ur => ur.IdReport);

        modelBuilder.Entity<UserReport>()
            .HasOne(ur => ur.Reporter)
            .WithMany(u => u.UserReportsMade)
            .HasForeignKey(ur => ur.IdUserReporter)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserReport>()
            .HasOne(ur => ur.Reported)
            .WithMany(u => u.UserReportsReceived)
            .HasForeignKey(ur => ur.IdUserReported)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  Session -> User
        // ======================================
        modelBuilder.Entity<Session>()
            .HasKey(s => s.IdSession);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.IdUser)
            .OnDelete(DeleteBehavior.SetNull);

        // ======================================
        //  RefreshToken -> User
        // ======================================
        modelBuilder.Entity<RefreshToken>()
            .HasKey(rt => rt.IdRefresh);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.IdUser)
            .OnDelete(DeleteBehavior.SetNull);

        // ======================================
        //  TinodeAccount -> User (1:1)
        // ======================================
        modelBuilder.Entity<TinodeAccount>()
            .HasKey(ta => ta.IdTinodeAccount);

        modelBuilder.Entity<TinodeAccount>()
            .HasOne(ta => ta.User)
            .WithOne(u => u.TinodeAccount)
            .HasForeignKey<TinodeAccount>(ta => ta.IdUser)
            .OnDelete(DeleteBehavior.SetNull);

        // ======================================
        //  AuditLog -> User
        // ======================================
        modelBuilder.Entity<AuditLog>()
            .HasKey(al => al.IdLog);

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.IdUser)
            .OnDelete(DeleteBehavior.SetNull);

        // ======================================
        //  Report (Table de jointure: User <-> UserReport)
        // ======================================
        modelBuilder.Entity<Report>()
            .HasKey(r => new { r.IdUser, r.IdReport });

        modelBuilder.Entity<Report>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.IdUser)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.UserReport)
            .WithMany(ur => ur.Reports)
            .HasForeignKey(r => r.IdReport)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  Blocage (Table de jointure: User <-> UserBlock)
        // ======================================
        modelBuilder.Entity<Blocage>()
            .HasKey(b => new { b.IdUser, b.IdBlock });

        modelBuilder.Entity<Blocage>()
            .HasOne(b => b.User)
            .WithMany(u => u.Blocages)
            .HasForeignKey(b => b.IdUser)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Blocage>()
            .HasOne(b => b.UserBlock)
            .WithMany(ub => ub.Blocages)
            .HasForeignKey(b => b.IdBlock)
            .OnDelete(DeleteBehavior.Restrict);

        // ======================================
        //  DATETIME UTC
        // ======================================
        modelBuilder.Entity<SubscriptionAppStripe>()
            .Property(s => s.StartSubscription)
            .HasColumnType("date");

        modelBuilder.Entity<SubscriptionAppStripe>()
            .Property(s => s.EndSubscription)
            .HasColumnType("date");

        modelBuilder.Entity<SubscriptionAppStripe>()
            .Property(s => s.CreatedAt)
            .HasColumnType("date");

        modelBuilder.Entity<SubscriptionCoachStripe>()
            .Property(s => s.StartSubscription)
            .HasColumnType("date");

        modelBuilder.Entity<SubscriptionCoachStripe>()
            .Property(s => s.EndSubscription)
            .HasColumnType("date");

        modelBuilder.Entity<SubscriptionCoachStripe>()
            .Property(s => s.CreatedAt)
            .HasColumnType("date");

        modelBuilder.Entity<Link>()
            .Property(l => l.StartDate)
            .HasColumnType("date");

        modelBuilder.Entity<Link>()
            .Property(l => l.EndDate)
            .HasColumnType("date");
    }
}


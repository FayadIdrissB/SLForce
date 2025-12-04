using Microsoft.EntityFrameworkCore;
using slf_backend.Entities;

namespace slf_backend.Data
{
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
        public DbSet<Athlete> Athletes => Set<Athlete>();
        public DbSet<Coach> Coaches => Set<Coach>();
        public DbSet<SubscriptionAppStripe> SubscriptionAppStripes => Set<SubscriptionAppStripe>();
        public DbSet<SubscriptionCoachStripe> SubscriptionCoachStripes => Set<SubscriptionCoachStripe>();
        public DbSet<Link> Links => Set<Link>();
        public DbSet<UserBlock> UserBlocks => Set<UserBlock>();
        public DbSet<UserReport> UserReports => Set<UserReport>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<TinodeAccount> TinodeAccounts => Set<TinodeAccount>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================
            //  DECIMAL PRECISION FIXES
            // ======================================
            modelBuilder.Entity<Coach>()
                .Property(c => c.MonthPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Coach>()
                .Property(c => c.Rating)
                .HasPrecision(3, 2);

            modelBuilder.Entity<SubscriptionAppStripe>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<SubscriptionCoachStripe>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);


            // ======================================
            //  Link (Athlete ↔ Coach Many-to-Many)
            // ======================================
            modelBuilder.Entity<Link>()
                .HasKey(l => new { l.AthleteId, l.CoachId });

            modelBuilder.Entity<Link>()
                .HasOne(l => l.Athlete)
                .WithMany()
                .HasForeignKey(l => l.AthleteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Link>()
                .HasOne(l => l.Coach)
                .WithMany()
                .HasForeignKey(l => l.CoachId)
                .OnDelete(DeleteBehavior.Restrict);


            // ======================================
            //  UserBlock (Bidirectional relations)
            // ======================================

            modelBuilder.Entity<UserBlock>()
                .HasOne(ub => ub.Blocker)
                .WithMany(u => u.UserBlocksMade)
                .HasForeignKey(ub => ub.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBlock>()
                .HasOne(ub => ub.Blocked)
                .WithMany(u => u.UserBlocksReceived)
                .HasForeignKey(ub => ub.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);


            // ======================================
            //  UserReport (Bidirectional relations)
            // ======================================

            modelBuilder.Entity<UserReport>()
                .HasOne(ur => ur.Reporter)
                .WithMany(u => u.UserReportsMade)
                .HasForeignKey(ur => ur.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserReport>()
                .HasOne(ur => ur.Reported)
                .WithMany(u => u.UserReportsReceived)
                .HasForeignKey(ur => ur.ReportedId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

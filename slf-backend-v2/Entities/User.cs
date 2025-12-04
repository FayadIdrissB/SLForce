using System;
using System.Collections.Generic;

namespace slf_backend.Entities
{
    public class User
    {
        public int Id { get; set; }

        // ===========================
        //   Basic User Info
        // ===========================
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Athlete / Coach
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ===========================
        //   Athlete / Coach Profiles
        // ===========================
        public Athlete? AthleteProfile { get; set; }
        public Coach? CoachProfile { get; set; }

        // ===========================
        //   Relations : BLOCKS
        // ===========================
        // Users that THIS USER has blocked
        public ICollection<UserBlock> UserBlocksMade { get; set; } = new List<UserBlock>();

        // Users that have blocked THIS USER
        public ICollection<UserBlock> UserBlocksReceived { get; set; } = new List<UserBlock>();

        // ===========================
        //   Relations : REPORTS
        // ===========================
        // Reports made BY this user
        public ICollection<UserReport> UserReportsMade { get; set; } = new List<UserReport>();

        // Reports made AGAINST this user
        public ICollection<UserReport> UserReportsReceived { get; set; } = new List<UserReport>();
    }
}

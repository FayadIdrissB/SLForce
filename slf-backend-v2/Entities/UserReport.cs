namespace slf_backend.Entities
{
    public class UserReport
    {
        public int Id { get; set; }

        public int ReporterId { get; set; }
        public int ReportedId { get; set; }

        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending";

        // Navigation
        public User? Reporter { get; set; }
        public User? Reported { get; set; }
    }
}

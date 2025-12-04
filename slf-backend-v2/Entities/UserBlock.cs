namespace slf_backend.Entities
{
    public class UserBlock
    {
        public int Id { get; set; }

        public int BlockerId { get; set; }
        public int BlockedId { get; set; }

        public bool Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation (optionnel)
        public User? Blocker { get; set; }
        public User? Blocked { get; set; }
    }
}

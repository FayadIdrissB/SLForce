namespace slf_backend.Entities;

public class Link
{
    public int AthleteId { get; set; }
    public int CoachId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Active";

    public User? Athlete { get; set; }
    public User? Coach { get; set; }
}

namespace slf_backend.DTO.Links;

public class LinkResponseDto
{
    public int AthleteId { get; set; }
    public int CoachId { get; set; }
    public string AthleteName { get; set; } = string.Empty;
    public string CoachName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}


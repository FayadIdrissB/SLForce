using System.ComponentModel.DataAnnotations;

namespace slf_backend.DTO.Links;

public class CreateLinkDto
{
    [Required(ErrorMessage = "L'ID de l'athlète est requis")]
    public int AthleteId { get; set; }

    [Required(ErrorMessage = "La date de début est requise")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "La date de fin est requise")]
    public DateTime EndDate { get; set; }
}


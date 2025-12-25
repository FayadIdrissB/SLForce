using System.ComponentModel.DataAnnotations;

namespace slf_backend.DTO.Moderation;

public class ReportUserDto
{
    [Required(ErrorMessage = "L'ID de l'utilisateur à signaler est requis")]
    public int UserIdToReport { get; set; }

    [Required(ErrorMessage = "La raison est requise")]
    [StringLength(200, ErrorMessage = "La raison ne peut pas dépasser 200 caractères")]
    public string Reason { get; set; } = string.Empty;
}


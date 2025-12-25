using System.ComponentModel.DataAnnotations;

namespace slf_backend.DTO.Moderation;

public class BlockUserDto
{
    [Required(ErrorMessage = "L'ID de l'utilisateur à bloquer est requis")]
    public int UserIdToBlock { get; set; }

    [StringLength(50, ErrorMessage = "La raison ne peut pas dépasser 50 caractères")]
    public string? Reason { get; set; }
}


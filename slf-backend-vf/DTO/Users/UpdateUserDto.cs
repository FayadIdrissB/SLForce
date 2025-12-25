using System.ComponentModel.DataAnnotations;

namespace slf_backend.DTO.Users;

public class UpdateUserDto
{
    [StringLength(20, ErrorMessage = "Le prénom ne peut pas dépasser 20 caractères")]
    public string? FirstName { get; set; }

    [StringLength(20, ErrorMessage = "Le nom ne peut pas dépasser 20 caractères")]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [StringLength(50, ErrorMessage = "L'email ne peut pas dépasser 50 caractères")]
    public string? Email { get; set; }
}


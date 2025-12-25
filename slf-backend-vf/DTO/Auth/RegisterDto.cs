using System.ComponentModel.DataAnnotations;

namespace slf_backend.DTO.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Le prénom est requis")]
    [StringLength(20, ErrorMessage = "Le prénom ne peut pas dépasser 20 caractères")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(20, ErrorMessage = "Le nom ne peut pas dépasser 20 caractères")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [StringLength(50, ErrorMessage = "L'email ne peut pas dépasser 50 caractères")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le rôle est requis")]
    [RegularExpression("^(Athlete|Coach)$", ErrorMessage = "Le rôle doit être 'Athlete' ou 'Coach'")]
    public string Role { get; set; } = string.Empty;
}


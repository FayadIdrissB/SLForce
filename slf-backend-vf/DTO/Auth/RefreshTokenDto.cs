using System.ComponentModel.DataAnnotations;

namespace slf_backend.DTO.Auth;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Le refresh token est requis")]
    public string RefreshToken { get; set; } = string.Empty;
}


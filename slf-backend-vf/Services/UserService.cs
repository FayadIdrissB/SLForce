using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.DTO.Users;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface IUserService
{
    Task<UserResponseDto> GetUserByIdAsync(int userId);
    Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto dto);
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.AthleteProfile)
            .Include(u => u.CoachProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"Utilisateur {userId} non trouvé");
        }

        var role = user.AthleteProfile != null ? "Athlete" : user.CoachProfile != null ? "Coach" : "Unknown";

        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role,
            IsAdmin = user.RoleAdmin
        };
    }

    public async Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var user = await _context.Users
            .Include(u => u.AthleteProfile)
            .Include(u => u.CoachProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"Utilisateur {userId} non trouvé");
        }

        if (!string.IsNullOrEmpty(dto.FirstName))
        {
            user.FirstName = dto.FirstName;
        }

        if (!string.IsNullOrEmpty(dto.LastName))
        {
            user.LastName = dto.LastName;
        }

        if (!string.IsNullOrEmpty(dto.Email))
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != userId);

            if (existingUser != null)
            {
                throw new Exception("Cet email est déjà utilisé par un autre utilisateur");
            }

            user.Email = dto.Email;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Utilisateur {UserId} mis à jour", userId);

        var role = user.AthleteProfile != null ? "Athlete" : user.CoachProfile != null ? "Coach" : "Unknown";

        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role,
            IsAdmin = user.RoleAdmin
        };
    }
}


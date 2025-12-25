using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using slf_backend.Data;
using slf_backend.DTO.Auth;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress, string? deviceType);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, string? ipAddress, string? deviceType);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IStripeService _stripeService;
    private readonly ITinodeService _tinodeService;
    private readonly IAuditService _auditService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IConfiguration configuration,
        IStripeService stripeService,
        ITinodeService tinodeService,
        IAuditService auditService,
        IPasswordHasher<User> passwordHasher,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _stripeService = stripeService;
        _tinodeService = tinodeService;
        _auditService = auditService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress, string? deviceType)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
        {
            throw new Exception("Un utilisateur avec cet email existe déjà");
        }

        var appSubscription = new SubscriptionAppStripe
        {
            IdStripe = "free_trial",
            StartSubscription = DateTime.UtcNow,
            EndSubscription = DateTime.UtcNow.AddMonths(1),
            StatusSubscription = "active",
            Price = 0,
            CreatedAt = DateTime.UtcNow
        };
        _context.SubscriptionAppStripes.Add(appSubscription);
        await _context.SaveChangesAsync();

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = string.Empty,
            RoleAdmin = false,
            SubscriptionAppStripeId = appSubscription.Id
        };

        user.Password = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (dto.Role == "Athlete")
        {
            var coachSubscription = new SubscriptionCoachStripe
            {
                IdStripe = "free_trial",
                StartSubscription = DateTime.UtcNow,
                EndSubscription = DateTime.UtcNow.AddMonths(1),
                StatusSubscription = "inactive",
                Price = 0,
                CreatedAt = DateTime.UtcNow
            };
            _context.SubscriptionCoachStripes.Add(coachSubscription);
            await _context.SaveChangesAsync();
            
            var athlete = new Athlete
            {
                UserId = user.Id,
                WeightCategory = "Non spécifié",
                CoachSubscriptionStripeId = coachSubscription.Id
            };
            _context.Athletes.Add(athlete);
        }
        else if (dto.Role == "Coach")
        {
            var coachSubscription = new SubscriptionCoachStripe
            {
                IdStripe = "free_trial",
                StartSubscription = DateTime.UtcNow,
                EndSubscription = DateTime.UtcNow.AddMonths(1),
                StatusSubscription = "inactive",
                Price = 0,
                CreatedAt = DateTime.UtcNow
            };
            _context.SubscriptionCoachStripes.Add(coachSubscription);
            await _context.SaveChangesAsync();
            
            var coach = new Coach
            {
                UserId = user.Id,
                MonthPrice = 0,
                Biography = string.Empty,
                Specialities = string.Empty,
                CompletedSessions = 0,
                Rating = 0,
                CoachSubscriptionStripeId = coachSubscription.Id
            };
            _context.Coaches.Add(coach);
        }

        await _context.SaveChangesAsync();

        await _tinodeService.CreateAccountAsync(user.Id);

        var session = new Session
        {
            IdUser = user.Id,
            DeviceType = deviceType ?? "Unknown",
            IpAdress = ipAddress ?? "Unknown",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _context.Sessions.Add(session);

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            IdUser = user.Id,
            RefreshTokenValue = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(90)
        };
        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(user.Id, "REGISTER", $"Inscription de l'utilisateur {user.Email}");

        _logger.LogInformation("Nouvel utilisateur enregistré: {Email}, ID: {UserId}", user.Email, user.Id);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = dto.Role,
                IsAdmin = user.RoleAdmin
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, string? ipAddress, string? deviceType)
    {
        var user = await _context.Users
            .Include(u => u.AthleteProfile)
            .Include(u => u.CoachProfile)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
        if (result != PasswordVerificationResult.Success)
        {
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
        }

        var session = new Session
        {
            IdUser = user.Id,
            DeviceType = deviceType ?? "Unknown",
            IpAdress = ipAddress ?? "Unknown",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _context.Sessions.Add(session);

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var oldTokens = await _context.RefreshTokens
            .Where(rt => rt.IdUser == user.Id)
            .ToListAsync();
        _context.RefreshTokens.RemoveRange(oldTokens);

        var refreshTokenEntity = new RefreshToken
        {
            IdUser = user.Id,
            RefreshTokenValue = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(90)
        };
        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(user.Id, "LOGIN", $"Connexion de l'utilisateur {user.Email}");

        var role = user.AthleteProfile != null ? "Athlete" : user.CoachProfile != null ? "Coach" : "Unknown";

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = role,
                IsAdmin = user.RoleAdmin
            }
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.AthleteProfile)
            .Include(rt => rt.User)
            .ThenInclude(u => u.CoachProfile)
            .FirstOrDefaultAsync(rt => rt.RefreshTokenValue == refreshToken);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token invalide ou expiré");
        }

        var user = tokenEntity.User!;
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        tokenEntity.RefreshTokenValue = newRefreshToken;
        tokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(90);

        await _context.SaveChangesAsync();

        var role = user.AthleteProfile != null ? "Athlete" : user.CoachProfile != null ? "Coach" : "Unknown";

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = role,
                IsAdmin = user.RoleAdmin
            }
        };
    }

    public async Task<bool> LogoutAsync(int userId)
    {
        var sessions = await _context.Sessions
            .Where(s => s.IdUser == userId)
            .ToListAsync();
        _context.Sessions.RemoveRange(sessions);

        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.IdUser == userId)
            .ToListAsync();
        _context.RefreshTokens.RemoveRange(refreshTokens);

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(userId, "LOGOUT", "Déconnexion de l'utilisateur");

        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var role = user.AthleteProfile != null ? "Athlete" : user.CoachProfile != null ? "Coach" : "Unknown";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim("IsAdmin", user.RoleAdmin.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}


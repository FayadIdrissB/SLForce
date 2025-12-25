using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface ITinodeService
{
    Task<string> CreateAccountAsync(int userId);
    Task<string?> GetTinodeUserIdAsync(int userId);
    Task<string> GenerateTinodeTokenAsync(int userId);
}

public class TinodeService : ITinodeService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TinodeService> _logger;

    public TinodeService(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<TinodeService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> CreateAccountAsync(int userId)
    {
        var existingAccount = await _context.TinodeAccounts
            .FirstOrDefaultAsync(ta => ta.IdUser == userId);

        if (existingAccount != null)
        {
            return existingAccount.TinodeUserId;
        }

        var tinodeUserId = $"user_{userId}_{Guid.NewGuid():N}";

        var tinodeAccount = new TinodeAccount
        {
            IdUser = userId,
            TinodeUserId = tinodeUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TinodeAccounts.Add(tinodeAccount);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Compte Tinode créé pour l'utilisateur {UserId}: {TinodeUserId}", userId, tinodeUserId);

        return tinodeUserId;
    }

    public async Task<string?> GetTinodeUserIdAsync(int userId)
    {
        var account = await _context.TinodeAccounts
            .FirstOrDefaultAsync(ta => ta.IdUser == userId);

        return account?.TinodeUserId;
    }

    public async Task<string> GenerateTinodeTokenAsync(int userId)
    {
        var tinodeUserId = await GetTinodeUserIdAsync(userId);
        
        if (string.IsNullOrEmpty(tinodeUserId))
        {
            tinodeUserId = await CreateAccountAsync(userId);
        }

        var apiKey = _configuration["Tinode:ApiKey"];
        var baseUrl = _configuration["Tinode:BaseUrl"];

        _logger.LogInformation("Token Tinode généré pour l'utilisateur {UserId}", userId);

        return $"tinode_token_{userId}_{DateTime.UtcNow.Ticks}";
    }
}


using Microsoft.EntityFrameworkCore;
using slf_backend.Data;
using slf_backend.DTO.Links;
using slf_backend.Entities;

namespace slf_backend.Services;

public interface ILinkService
{
    Task<LinkResponseDto> CreateLinkAsync(int coachUserId, CreateLinkDto dto);
    Task<List<LinkResponseDto>> GetMyAthletesAsync(int coachUserId);
    Task<LinkResponseDto?> GetMyCoachAsync(int athleteUserId);
    Task<bool> CancelLinkAsync(int athleteEntityId, int coachEntityId);
}

public class LinkService : ILinkService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LinkService> _logger;

    public LinkService(AppDbContext context, ILogger<LinkService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LinkResponseDto> CreateLinkAsync(int coachUserId, CreateLinkDto dto)
    {
        var coach = await _context.Coaches
            .Include(c => c.User)
            .Include(c => c.CoachSubscriptionStripe)
            .FirstOrDefaultAsync(c => c.UserId == coachUserId);

        if (coach == null)
        {
            throw new Exception("Coach non trouvé");
        }

        if (coach.CoachSubscriptionStripe.StatusSubscription != "active")
        {
            throw new Exception("L'abonnement coach n'est pas actif");
        }

        var athlete = await _context.Athletes
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == dto.AthleteId);

        if (athlete == null)
        {
            throw new Exception("Athlète non trouvé");
        }

        var existingLink = await _context.Links
            .FirstOrDefaultAsync(l => l.IdUserAthlete == dto.AthleteId && l.IdUserCoach == coach.Id);

        if (existingLink != null)
        {
            throw new Exception("Un lien existe déjà entre ce coach et cet athlète");
        }

        if (dto.StartDate >= dto.EndDate)
        {
            throw new Exception("La date de début doit être antérieure à la date de fin");
        }

        var link = new Link
        {
            IdUserAthlete = dto.AthleteId,
            IdUserCoach = coach.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "Active"
        };

        _context.Links.Add(link);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Lien créé entre le coach {CoachId} et l'athlète {AthleteId}", coachUserId, dto.AthleteId);

        return new LinkResponseDto
        {
            AthleteId = dto.AthleteId,
            CoachId = coach.Id,
            AthleteName = $"{athlete.User.FirstName} {athlete.User.LastName}",
            CoachName = $"{coach.User.FirstName} {coach.User.LastName}",
            StartDate = link.StartDate,
            EndDate = link.EndDate,
            Status = link.Status
        };
    }

    public async Task<List<LinkResponseDto>> GetMyAthletesAsync(int coachUserId)
    {
        var coach = await _context.Coaches
            .FirstOrDefaultAsync(c => c.UserId == coachUserId);

        if (coach == null)
        {
            throw new Exception("Coach non trouvé");
        }

        var links = await _context.Links
            .Include(l => l.Athlete)
            .ThenInclude(a => a.User)
            .Include(l => l.Coach)
            .ThenInclude(c => c.User)
            .Where(l => l.IdUserCoach == coach.Id && l.Status == "Active")
            .ToListAsync();

        return links.Select(l => new LinkResponseDto
        {
            AthleteId = l.IdUserAthlete,
            CoachId = l.IdUserCoach,
            AthleteName = $"{l.Athlete.User.FirstName} {l.Athlete.User.LastName}",
            CoachName = $"{l.Coach.User.FirstName} {l.Coach.User.LastName}",
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            Status = l.Status
        }).ToList();
    }

    public async Task<LinkResponseDto?> GetMyCoachAsync(int athleteUserId)
    {
        var athlete = await _context.Athletes
            .FirstOrDefaultAsync(a => a.UserId == athleteUserId);

        if (athlete == null)
        {
            throw new Exception("Athlète non trouvé");
        }

        var link = await _context.Links
            .Include(l => l.Athlete)
            .ThenInclude(a => a.User)
            .Include(l => l.Coach)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(l => l.IdUserAthlete == athlete.Id && l.Status == "Active");

        if (link == null)
        {
            return null;
        }

        return new LinkResponseDto
        {
            AthleteId = link.IdUserAthlete,
            CoachId = link.IdUserCoach,
            AthleteName = $"{link.Athlete.User.FirstName} {link.Athlete.User.LastName}",
            CoachName = $"{link.Coach.User.FirstName} {link.Coach.User.LastName}",
            StartDate = link.StartDate,
            EndDate = link.EndDate,
            Status = link.Status
        };
    }

    public async Task<bool> CancelLinkAsync(int athleteEntityId, int coachEntityId)
    {
        var link = await _context.Links
            .FirstOrDefaultAsync(l => l.IdUserAthlete == athleteEntityId && l.IdUserCoach == coachEntityId);

        if (link == null)
        {
            throw new Exception("Lien non trouvé");
        }

        link.Status = "Cancelled";
        await _context.SaveChangesAsync();

        _logger.LogInformation("Lien annulé entre le coach {CoachId} et l'athlète {AthleteId}", coachEntityId, athleteEntityId);

        return true;
    }
}


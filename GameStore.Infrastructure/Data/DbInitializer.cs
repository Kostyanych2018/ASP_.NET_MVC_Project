using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.Infrastructure.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        ApplicationDbContext context,
        ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.MigrateAsync(cancellationToken);
            
            await SeedGenresAsync(cancellationToken);
            await SeedGamesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing or seeding the database.");
            throw;
        }
    }

    private async Task SeedGenresAsync(CancellationToken cancellationToken)
    {
        if (await _context.Genres.AnyAsync(cancellationToken))
        {
            return;
        }

        var genres = new List<Genre>
        {
            new() { Name = "Strategy", NormalizedName = "strategies" },
            new() { Name = "Role-Playing Games (RPG)", NormalizedName = "rpg" },
            new() { Name = "Survival", NormalizedName = "survival" },
            new() { Name = "Shooter", NormalizedName = "shooter" }
        };
        await _context.Genres.AddRangeAsync(genres, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Genres seeded successfully.");
    }

    private async Task SeedGamesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Games.AnyAsync(cancellationToken))
        {
            return;
        }

        var strategiesGenre = await _context.Genres.FirstOrDefaultAsync(g => g.NormalizedName == "strategies", cancellationToken);
        var rpgGenre = await _context.Genres.FirstOrDefaultAsync(g => g.NormalizedName == "rpg", cancellationToken);
        var survivalGenre = await _context.Genres.FirstOrDefaultAsync(g => g.NormalizedName == "survival", cancellationToken);
        var shooterGenre = await _context.Genres.FirstOrDefaultAsync(g => g.NormalizedName == "shooter", cancellationToken);

        if (strategiesGenre == null || rpgGenre == null || survivalGenre == null || shooterGenre == null)
        {
            return;
        }

        var games = new List<Game>
        {
            new()
            {
                Name = "Total War: Warhammer III",
                Description = "Real-time strategy game",
                Price = 149.99m,
                Image = "Images/totalwar.png",
                GenreId = strategiesGenre.Id
            },
            new()
            {
                Name = "Civilization VI",
                Description = "Turn-based civilization building strategy",
                Price = 119.99m,
                Image = "Images/civilizationvi.png",
                GenreId = strategiesGenre.Id
            },
            new()
            {
                Name = "Stellaris",
                Description = "Grand space strategy game",
                Price = 109.99m,
                Image = "Images/stellaris.png",
                GenreId = strategiesGenre.Id
            },
            new()
            {
                Name = "Elden Ring",
                Description = "Open-world action RPG",
                Price = 159.99m,
                Image = "Images/eldenring.jpg",
                GenreId = rpgGenre.Id
            },
            new()
            {
                Name = "Cyberpunk 2077",
                Description = "Sci-fi role-playing game",
                Price = 144.99m,
                Image = "Images/cyberpunk2077.jpg",
                GenreId = rpgGenre.Id
            },
            new()
            {
                Name = "Valheim",
                Description = "Viking-themed survival game",
                Price = 99.99m,
                Image = "Images/valheim.png",
                GenreId = survivalGenre.Id
            },
            new()
            {
                Name = "The Forest",
                Description = "Survival on a cannibal-infested island",
                Price = 49.99m,
                Image = "Images/theforest.jpg",
                GenreId = survivalGenre.Id
            },
            new()
            {
                Name = "DOOM Eternal",
                Description = "Fast-paced demon-slaying shooter",
                Price = 124.99m,
                Image = "Images/doometernal.png",
                GenreId = shooterGenre.Id
            },
            new()
            {
                Name = "Counter-Strike 2",
                Description = "Team-based tactical shooter",
                Price = 0m,
                Image = "Images/cs2.jpeg",
                GenreId = shooterGenre.Id
            }
        };

        await _context.Games.AddRangeAsync(games, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Games seeded successfully.");
    }
}
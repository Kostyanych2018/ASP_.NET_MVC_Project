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
            new() { Name = "Стратегии", NormalizedName = "strategies" },
            new() { Name = "Ролевые игры (RPG)", NormalizedName = "rpg" },
            new() { Name = "Выживание", NormalizedName = "survival" },
            new() { Name = "Шутер", NormalizedName = "shooter" }
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
                Description = "Стратегия в реальном времени",
                Price = 149.99m,
                Image = "Images/totalwar.png",
                GenreId = strategiesGenre.Id
            },
            new()
            {
                Name = "Civilization VI",
                Description = "Пошаговая стратегия о развитии цивилизации",
                Price = 119.99m,
                Image = "Images/civilizationvi.png",
                GenreId = strategiesGenre.Id
            },
            new()
            {
                Name = "Stellaris",
                Description = "Космическая глобальная стратегия",
                Price = 109.99m,
                Image = "Images/stellaris.png",
                GenreId = strategiesGenre.Id
            },
            new()
            {
                Name = "Elden Ring",
                Description = "Экшн-РПГ с открытым миром",
                Price = 159.99m,
                Image = "Images/eldenring.jpg",
                GenreId = rpgGenre.Id
            },
            new()
            {
                Name = "Cyberpunk 2077",
                Description = "Научно-фантастическая РПГ",
                Price = 144.99m,
                Image = "Images/cyberpunk2077.jpg",
                GenreId = rpgGenre.Id
            },
            new()
            {
                Name = "Valheim",
                Description = "Выживание в скандинавском стиле",
                Price = 99.99m,
                Image = "Images/valheim.png",
                GenreId = survivalGenre.Id
            },
            new()
            {
                Name = "The Forest",
                Description = "Выживание на острове с каннибалами",
                Price = 49.99m,
                Image = "Images/theforest.jpg",
                GenreId = survivalGenre.Id
            },
            new()
            {
                Name = "DOOM Eternal",
                Description = "Динамичный шутер против демонов",
                Price = 124.99m,
                Image = "Images/doometernal.png",
                GenreId = shooterGenre.Id
            },
            new()
            {
                Name = "Counter-Strike 2",
                Description = "Командный тактический шутер",
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
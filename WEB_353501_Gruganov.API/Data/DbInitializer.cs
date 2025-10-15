using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.API.Data;

public class DbInitializer
{
    public static async Task SeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var configuration = app.Configuration;
        var baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");

        if (await context.Games.AnyAsync() || await context.Genres.AnyAsync()) {
            return;
        }

        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Стратегии", NormalizedName = "strategies" },
            new() { Id = 2, Name = "Ролевые игры (RPG)", NormalizedName = "rpg" },
            new() { Id = 3, Name = "Выживание", NormalizedName = "survival" },
            new() { Id = 4, Name = "Шутер", NormalizedName = "shooter" }
        };
        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();

        var games = new List<Game>
        {
            new()
            {
                Id = 1, Name = "Total War: Warhammer III",
                Description = "Стратегия в реальном времени",
                Price = 149.99m, Image = baseUrl + "/Images/totalwar.png",
                Genre = genres.Find(g => g.NormalizedName.Equals("strategies"))
            },
            new()
            {
                Id = 2, Name = "Civilization VI",
                Description = "Пошаговая стратегия о развитии цивилизации",
                Price = 119.99m, Image = baseUrl + "/Images/civilizationvi.png",
                Genre = genres.Find(g => g.NormalizedName.Equals("strategies"))
            },
            new()
            {
                Id = 3, Name = "Stellaris",
                Description = "Космическая глобальная стратегия",
                Price = 109.99m, Image = baseUrl + "/Images/stellaris.png",
                Genre = genres.Find(g => g.NormalizedName.Equals("strategies"))
            },
            new()
            {
                Id = 4, Name = "Elden Ring",
                Description = "Экшн-РПГ с открытым миром",
                Price = 159.99m, Image = baseUrl + "/Images/eldenring.jpg",
                Genre = genres.Find(g => g.NormalizedName.Equals("rpg"))
            },
            new()
            {
                Id = 5, Name = "Cyberpunk 2077",
                Description = "Научно-фантастическая РПГ",
                Price = 144.99m, Image = baseUrl + "/Images/cyberpunk2077.jpg",
                Genre = genres.Find(g => g.NormalizedName.Equals("rpg"))
            },
            new()
            {
                Id = 6, Name = "Valheim",
                Description = "Выживание в скандинавском стиле",
                Price = 99.99m, Image = baseUrl + "/Images/valheim.png",
                Genre = genres.Find(g => g.NormalizedName.Equals("survival"))
            },
            new()
            {
                Id = 7, Name = "The Forest",
                Description = "Выживание на острове с каннибалами",
                Price = 49.99m, Image = baseUrl + "/Images/theforest.jpg",
                Genre = genres.Find(g => g.NormalizedName.Equals("survival"))
            },
            new()
            {
                Id = 8, Name = "DOOM Eternal",
                Description = "Динамичный шутер против демонов",
                Price = 124.99m, Image = baseUrl + "/Images/doometernal.png",
                Genre = genres.Find(g => g.NormalizedName.Equals("shooter"))
            },
            new()
            {
                Id = 9, Name = "Counter-Strike 2",
                Description = "Командный тактический шутер",
                Price = 0m, Image = baseUrl + "/Images/cs2.jpeg",
                Genre = genres.Find(g => g.NormalizedName.Equals("shooter"))
            },
        };

        await context.Games.AddRangeAsync(games);
        await context.SaveChangesAsync();
    }
}
using Microsoft.AspNetCore.Mvc;

namespace WEB_353501_Gruganov.UI.Services.GameService;

public class MemoryGameService : IGameService
{
    private List<Game> _games;
    private List<Genre> _genres;
    private IConfiguration _configuration;

    public MemoryGameService([FromServices] IConfiguration configuration,
        IGenreService genreService)
    {
        _genres = genreService.GetGenresListAsync()
            .Result
            .Data;
        _configuration = configuration;
        SetupData();
    }

    private void SetupData()
    {
        _games = new List<Game>
        {
            new()
            {
                Id = 1, Name = "Total War: Warhammer III",
                Description = "Стратегия в реальном времени",
                Price = 149.99m, Image = "../Images/totalwar.png",
                Genre = _genres.Find(g => g.NormalizedName.Equals("strategies"))
            },
            new()
            {
                Id = 2, Name = "Civilization VI",
                Description = "Пошаговая стратегия о развитии цивилизации",
                Price = 119.99m, Image = "../Images/civilizationvi.png",
                Genre = _genres.Find(g => g.NormalizedName.Equals("strategies"))
            },
            new()
            {
                Id = 3, Name = "Stellaris",
                Description = "Космическая глобальная стратегия",
                Price = 109.99m, Image = "../Images/stellaris.png",
                Genre = _genres.Find(g => g.NormalizedName.Equals("strategies"))
            },
            new()
            {
                Id = 4, Name = "Elden Ring",
                Description = "Экшн-РПГ с открытым миром",
                Price = 159.99m, Image = "../Images/eldenring.jpg",
                Genre = _genres.Find(g => g.NormalizedName.Equals("rpg"))
            },
            new()
            {
                Id = 5, Name = "Cyberpunk 2077",
                Description = "Научно-фантастическая РПГ",
                Price = 144.99m, Image = "../Images/cyberpunk2077.jpg",
                Genre = _genres.Find(g => g.NormalizedName.Equals("rpg"))
            },
            new()
            {
                Id = 6, Name = "Valheim",
                Description = "Выживание в скандинавском стиле",
                Price = 99.99m, Image = "../Images/valheim.png",
                Genre = _genres.Find(g => g.NormalizedName.Equals("survival"))
            },
            new()
            {
                Id = 7, Name = "The Forest",
                Description = "Выживание на острове с каннибалами",
                Price = 49.99m, Image = "../Images/theforest.jpg",
                Genre = _genres.Find(g => g.NormalizedName.Equals("survival"))
            },
            new()
            {
                Id = 8, Name = "DOOM Eternal",
                Description = "Динамичный шутер против демонов",
                Price = 124.99m, Image = "../Images/doometernal.png",
                Genre = _genres.Find(g => g.NormalizedName.Equals("shooter"))
            },
            new()
            {
                Id = 9, Name = "Counter-Strike 2",
                Description = "Командный тактический шутер",
                Price = 0m, Image = "../Images/cs2.jpeg",
                Genre = _genres.Find(g => g.NormalizedName.Equals("shooter"))
            },
        };
    }

    public Task<ResponseData<ListModel<Game>>> GetGamesListAsync(string? genreNormalizedName, int pageNo = 1)
    {
        var filteredGames = _games;

        if (!string.IsNullOrEmpty(genreNormalizedName)) {
            filteredGames = filteredGames
                .Where(g => g.Genre?.NormalizedName == genreNormalizedName)
                .ToList();
        }

        int itemsPerPage = _configuration.GetValue<int>("ItemsPerPage");
        int totalItems = filteredGames.Count();
        int totalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
        pageNo = Math.Max(1, Math.Min(totalPages, pageNo));
        filteredGames = filteredGames
            .Skip((pageNo - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToList();
        
        var listModel = new ListModel<Game>()
        {
            Items = filteredGames,
            CurrentPage = pageNo,
            TotalPages = totalPages
        };

        var result = ResponseData<ListModel<Game>>.Success(listModel);
        return Task.FromResult(result);
    }

    public Task<ResponseData<Game>> GetGameByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateGameAsync(int id, Game game, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGameAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseData<Game>> CreateGameAsync(Game game, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }
}
using WEB_353501_Gruganov.Domain.Entities;

namespace WEB_353501_Gruganov.BlazorWasm.Services;

public interface IDataService
{
    event Action DataLoaded;

    List<Genre> Genres { get; set; }
    List<Game> Games { get; set; }

    bool Success { get; set; }
    string ErrorMessage { get; set; }

    int TotalPages { get; set; }
    int CurrentPage { get; set; }

    Genre? SelectedGenre { get; set; }

    Task GetGamesListAsync(int pageNo = 1);

    Task GetGenresListAsync();
}
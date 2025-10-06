namespace WEB_353501_Gruganov.UI.Services.GenreService;

public class MemoryGenreService : IGenreService
{
    public Task<ResponseData<List<Genre>>> GetGenresListAsync()
    {
        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Стратегии",NormalizedName = "strategies"},
            new() { Id = 2, Name = "Ролевые игры (RPG)",NormalizedName = "rpg" },
            new() { Id = 3, Name = "Выживание", NormalizedName = "survival"},
            new() { Id = 4, Name = "Шутер",NormalizedName = "shooter"}
        };
        var result = ResponseData<List<Genre>>.Success(genres);
        return Task.FromResult(result);
    }
}
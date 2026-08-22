namespace GameStore.UI.Services.GenreService;
public interface IGenreService
{
    public Task<ResponseData<List<Genre>>> GetGenresListAsync();
}
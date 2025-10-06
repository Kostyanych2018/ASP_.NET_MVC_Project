namespace WEB_353501_Gruganov.UI.Services.GenreService;
public interface IGenreService
{
    public Task<ResponseData<List<Genre>>> GetGenresListAsync();
}
namespace WEB_353501_Gruganov.UI.Services.GameService;

public interface IGameService
{
    public Task<ResponseData<ListModel<Game>>> GetGamesListAsync(string?
        genreNormalizedName, int pageNo=1);
    
    public Task<ResponseData<Game>> GetGameByIdAsync(int id);
    
    public Task UpdateGameAsync(int id, Game game,IFormFile? formFile);
    
    public Task DeleteGameAsync(int id);
    
    public Task<ResponseData<Game>> CreateGameAsync(Game game,IFormFile? formFile);
}
namespace WEB_353501_Gruganov.UI.Services.GameService;

public interface IGameService
{
    public Task<ResponseData<ListModel<Game>>> GetGamesListAsync(string?
        genreNormalizedName, int pageNo, int? pageSize);
    
    public Task<ResponseData<Game>> GetGameByIdAsync(int id);
    
    public Task<ResponseData<Game>> UpdateGameAsync(int id, Game game,IFormFile? formFile);
    
    public Task<ResponseData<Game>> DeleteGameAsync(int id);
    
    public Task<ResponseData<Game>> CreateGameAsync(Game game,IFormFile? formFile);
}
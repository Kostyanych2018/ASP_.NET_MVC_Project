using GameStore.Application.Games.DTOs;

namespace GameStore.UI.Services.Games;

public interface IGameService
{
    public Task<ListModel<GameDto>> GetGamesListAsync(
        string? genreNormalizedName,
        int pageNo = 1,
        int? pageSize = null,
        CancellationToken cancellationToken = default);

    public Task<GameDto> GetGameByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    public Task<GameDto> CreateGameAsync(
        GameDto game,
        IFormFile? formFile,
        CancellationToken cancellationToken = default);

    public Task<GameDto> UpdateGameAsync(
        int id,
        GameDto game,
        IFormFile? formFile,
        CancellationToken cancellationToken = default);

    public Task DeleteGameAsync(
        int id,
        CancellationToken cancellationToken = default);
}
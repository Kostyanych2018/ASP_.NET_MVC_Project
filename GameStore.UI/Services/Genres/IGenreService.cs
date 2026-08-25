using GameStore.Application.Genres.DTOs;

namespace GameStore.UI.Services.Genres;
public interface IGenreService
{
    public Task<List<GenreDto>> GetGenresListAsync(CancellationToken  cancellationToken = default);
}
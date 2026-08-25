using GameStore.Application.Genres.DTOs;
using GameStore.UI.Extensions;

namespace GameStore.UI.Services.Genres;

public class ApiGenreService : IGenreService
{
    private readonly HttpClient _httpClient;

    public ApiGenreService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GenreDto>> GetGenresListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(string.Empty, cancellationToken);
        return await response.ReadAsJsonOrThrowAsync<List<GenreDto>>(cancellationToken); 
    }
}
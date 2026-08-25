using System.Globalization;
using System.Net.Http.Headers;
using GameStore.Application.Games.DTOs;
using GameStore.UI.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace GameStore.UI.Services.Games;

public class ApiGameService : IGameService
{
    private readonly int _defaultPageSize;
    private readonly HttpClient _httpClient;

    public ApiGameService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _defaultPageSize = configuration.GetValue<int>("ItemsPerPage");
    }


    public async Task<ListModel<GameDto>> GetGamesListAsync(
        string? genreNormalizedName,
        int pageNo = 1,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNo"] = pageNo.ToString(),
            ["pageSize"] = (pageSize ?? _defaultPageSize).ToString()
        };

        var requestUri = string.IsNullOrWhiteSpace(genreNormalizedName)
            ? QueryHelpers.AddQueryString(string.Empty, queryParams)
            : QueryHelpers.AddQueryString($"{genreNormalizedName}", queryParams);

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        return await response.ReadAsJsonOrThrowAsync<ListModel<GameDto>>(cancellationToken);
    }

    public async Task<GameDto> GetGameByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{id}", cancellationToken);
        return await response.ReadAsJsonOrThrowAsync<GameDto>(cancellationToken);
    }

    public async Task<GameDto> CreateGameAsync(
        GameDto game,
        IFormFile? formFile,
        CancellationToken cancellationToken = default)
    {
        using var content = CreateMultipartFormData(game, formFile);
        var response = await _httpClient.PostAsync(string.Empty, content, cancellationToken);
        return await response.ReadAsJsonOrThrowAsync<GameDto>(cancellationToken);
    }

    public async Task<GameDto> UpdateGameAsync(
        int id,
        GameDto game,
        IFormFile? formFile,
        CancellationToken cancellationToken = default)
    {
        using var content = CreateMultipartFormData(game, formFile);
        var response = await _httpClient.PutAsync($"{id}", content, cancellationToken);
        return await response.ReadAsJsonOrThrowAsync<GameDto>(cancellationToken);
    }

    public async Task DeleteGameAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{id}", cancellationToken);
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);
    }

    private static MultipartFormDataContent CreateMultipartFormData(GameDto game, IFormFile? formFile)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(game.Name), nameof(GameDto.Name) },
            { new StringContent(game.Description ?? string.Empty), nameof(GameDto.Description) },
            { new StringContent(game.Price.ToString(CultureInfo.InvariantCulture)), nameof(GameDto.Price) },
            { new StringContent(game.GenreId.ToString(CultureInfo.InvariantCulture)), nameof(GameDto.GenreId) }
        };

        if (formFile == null)
        {
            return content;
        }

        var streamContent = new StreamContent(formFile.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
        content.Add(streamContent, "formFile", formFile.FileName);

        return content;
    }
}
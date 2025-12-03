using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using WEB_353501_Gruganov.UI.Services.Authentication;

namespace WEB_353501_Gruganov.UI.Services.GameService;

public class ApiGameService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<ApiGameService> logger,
    ITokenAccessor tokenAccessor) : IGameService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;
    private readonly int _pageSize = configuration.GetValue<int>("ItemsPerPage");

    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<ResponseData<ListModel<Game>>> GetGamesListAsync(
        string? genreNormalizedName,
        int pageNo = 1,
        int? pageSize = null)
    {
        var urlString = new StringBuilder($"{_httpClient.BaseAddress!.AbsoluteUri}");
        if (!string.IsNullOrEmpty(genreNormalizedName)) {
            urlString.Append($"/{genreNormalizedName}");
        }

        var queryParams = new List<string>();


        if (pageNo > 1) {
            queryParams.Add($"pageNo={pageNo}");        
        }

        var finalPageSize = pageSize ?? _pageSize;
        queryParams.Add($"pageSize={finalPageSize}");

        if (queryParams.Any()) {
            urlString.Append($"?{string.Join("&", queryParams)}");
        }

        var response = await _httpClient.GetAsync(urlString.ToString());

        if (response.IsSuccessStatusCode) {
            try {
                return await response
                    .Content
                    .ReadFromJsonAsync<ResponseData<ListModel<Game>>>(_serializerOptions);
            }
            catch (JsonException ex) {
                _logger.LogError($"-----> Ошибка: {ex.Message}");
                return ResponseData<ListModel<Game>>.Error($"Ошибка: {ex.Message}");
            }
        }

        _logger.LogError($"-----> Данные не получены от сервера. Ошибка: {response.StatusCode.ToString()}");
        return ResponseData<ListModel<Game>>.Error($"Данные не получены от сервера. Ошибка: {response.StatusCode.ToString()}");
    }


    public async Task<ResponseData<Game>> GetGameByIdAsync(int id)
    {
        var urlString = $"{_httpClient.BaseAddress.AbsoluteUri}/{id}";

        try {
            await tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
        }
        catch (Exception ex) {
            return ResponseData<Game>.Error($"Игра с id:{id} не получена. Ошибка: {ex.Message}");
        }

        var response = await _httpClient.GetAsync(urlString);

        if (response.IsSuccessStatusCode) {
            try {
                return await response
                    .Content
                    .ReadFromJsonAsync<ResponseData<Game>>(_serializerOptions);
            }
            catch (JsonException ex) {
                _logger.LogError($"-----> Ошибка десериализации (ID={id}): {ex.Message}");
                return ResponseData<Game>.Error($"Ошибка: {ex.Message}");
            }
        }

        _logger.LogError($"-----> Данные не получены от сервера. Ошибка: {response.StatusCode}");
        return ResponseData<Game>.Error($"Данные не получены от сервера. Ошибка: {response.StatusCode}");
    }

    public async Task<ResponseData<Game>> CreateGameAsync(Game game, IFormFile? formFile)
    {
        var content = new MultipartFormDataContent();

        var gameContent = new StringContent(
            JsonSerializer.Serialize(game, _serializerOptions),
            Encoding.UTF8,
            "application/json");

        content.Add(gameContent, "game");
        if (formFile != null) {
            var fileContent = new StreamContent(formFile.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            content.Add(fileContent, "file", formFile.FileName);
        }

        try {
            await tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
        }
        catch (Exception ex) {
            return ResponseData<Game>.Error($"Игра не создана. Ошибка: {ex.Message}");
        }

        var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
        if (response.IsSuccessStatusCode) {
            try {
                return await response.Content.ReadFromJsonAsync<ResponseData<Game>>(_serializerOptions);
            }
            catch (JsonException ex) {
                _logger.LogError($"-----> Ошибка десериализации при создании: {ex.Message}");
                return ResponseData<Game>.Error($"Ошибка: {ex.Message}");
            }
        }

        _logger.LogError($"-----> Не удалось создать игру. Ошибка: {response.StatusCode}");
        return ResponseData<Game>.Error($"Не удалось создать игру. Ошибка: {response.StatusCode}");
    }

    public async Task<ResponseData<Game>> UpdateGameAsync(int id, Game game, IFormFile? formFile)
    {
        var content = new MultipartFormDataContent();

        var gameContent = new StringContent(
            JsonSerializer.Serialize(game, _serializerOptions),
            Encoding.UTF8,
            "application/json");

        content.Add(gameContent, "game");

        if (formFile != null) {
            var fileContent = new StreamContent(formFile.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
            content.Add(fileContent, "image", formFile.FileName);
        }

        try {
            await tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
        }
        catch (Exception ex) {
            return ResponseData<Game>.Error($"Игра {game.Name} не обновлена. Ошибка: {ex.Message}");
        }

        var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{id}", content);
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Не удалось обновить игру. Статус: {response.StatusCode}. Ошибка: {error}");
            return ResponseData<Game>.Error($"Не удалось обновить игру. Ошибка: {response.StatusCode}");
        }

        _logger.LogInformation($"Игра с id:{game.Id} обновлена.");
        return ResponseData<Game>.Success($"Игра с id:{game.Id} обновлена.");
    }

    public async Task<ResponseData<Game>> DeleteGameAsync(int id)
    {
        try {
            await tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
        }
        catch (Exception ex) {
            return ResponseData<Game>.Error($"Игра с id:{id} не удалена. Ошибка: {ex.Message}");
        }

        var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{id}");
        
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Не удалось удалить игру. Статус: {response.StatusCode}. Ошибка: {error}");
        }

        _logger.LogInformation($"Игра с id:{id} удалена.");
        return ResponseData<Game>.Success($"Игра с id:{id} удалена.");
    }
}
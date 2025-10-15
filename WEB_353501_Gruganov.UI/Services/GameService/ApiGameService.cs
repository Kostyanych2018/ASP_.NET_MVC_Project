using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

namespace WEB_353501_Gruganov.UI.Services.GameService;

public class ApiGameService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<ApiGameService> logger) : IGameService
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
        int pageNo = 1)
    {
        var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}");
        if (!string.IsNullOrEmpty(genreNormalizedName)) {
            urlString.Append($"/{genreNormalizedName}");
        }

        var queryParams = new List<string>();
    
        if (pageNo > 1) {
            queryParams.Add($"pageNo={pageNo}");
        }
        
        if (_pageSize != 3) { 
            queryParams.Add($"pageSize={_pageSize}");
        }

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
        var urlString = $"{_httpClient.BaseAddress.AbsoluteUri}{id}";
        var response = await _httpClient.GetAsync(urlString);
        
        if (response.IsSuccessStatusCode)
        {
            try
            {
                return await response
                    .Content
                    .ReadFromJsonAsync<ResponseData<Game>>(_serializerOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"-----> Ошибка десериализации (ID={id}): {ex.Message}");
                return ResponseData<Game>.Error($"Ошибка: {ex.Message}");
            }
        }
        
        _logger.LogError($"-----> Данные не получены от сервера. Ошибка: {response.StatusCode}");
        return ResponseData<Game>.Error($"Данные не получены от сервера. Ошибка: {response.StatusCode}");
    }

    public Task UpdateGameAsync(int id, Game game, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGameAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseData<Game>> CreateGameAsync(Game game, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }
}
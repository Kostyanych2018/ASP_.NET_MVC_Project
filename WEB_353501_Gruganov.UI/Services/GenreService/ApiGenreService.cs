using System.Text;
using System.Text.Json;
using WEB_353501_Gruganov.UI.Services.GameService;

namespace WEB_353501_Gruganov.UI.Services.GenreService;

public class ApiGenreService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<ApiGameService> logger): IGenreService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;
    // private readonly int _pageSize = configuration.GetValue<int>("ItemsPerPage");

    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public async Task<ResponseData<List<Genre>>> GetGenresListAsync()
    {
        var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}");
        
        var response = await _httpClient.GetAsync(urlString.ToString());
        
        if (response.IsSuccessStatusCode) {
            try {
                var genres = await response.Content.ReadFromJsonAsync<List<Genre>>(_serializerOptions);
                return ResponseData<List<Genre>>.Success(genres ?? []);
            }
            catch (JsonException ex) {
                _logger.LogError($"-----> Ошибка: {ex.Message}");
                return ResponseData<List<Genre>>.Error($"Ошибка: {ex.Message}");
            }
        }
        _logger.LogError($"-----> Данные не получены от сервера. Ошибка: {response.StatusCode.ToString()}");
        return ResponseData<List<Genre>>.Error($"Данные не получены от сервера. Ошибка: {response.StatusCode.ToString()}");
    }
}
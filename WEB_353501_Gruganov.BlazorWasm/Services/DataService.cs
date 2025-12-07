using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using WEB_353501_Gruganov.Domain.Entities;
using WEB_353501_Gruganov.Domain.Models;

namespace WEB_353501_Gruganov.BlazorWasm.Services;

public class DataService : IDataService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly int _pageSize;

    public DataService(HttpClient httpClient,
        IConfiguration configuration,
        IAccessTokenProvider accessTokenProvider)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _accessTokenProvider = accessTokenProvider;
        _pageSize = _configuration.GetValue<int>("Api:PageSize");
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        Genres = [];
        Games = [];
    }

    public event Action? DataLoaded;
    public List<Genre> Genres { get; set; }
    public List<Game> Games { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public Genre? SelectedGenre { get; set; }

    private async Task<string> GetJwtTokenAsync()
    {
        var tokenRequest = await _accessTokenProvider.RequestAccessToken();
        if (tokenRequest.TryGetToken(out var token)) {
            return token.Value;
        }
        throw new UnauthorizedAccessException("Не удалось получить токен авторизации. Пользователь не авторизован.");
    }

    private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string uri)
    {
        var token = await GetJwtTokenAsync();
        var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
    
    public async Task GetGenresListAsync()
    {
        try {
            var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "genres");
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode) {
                try {
                    var genres = await response.Content
                        .ReadFromJsonAsync<List<Genre>>(_serializerOptions);

                    if (genres != null) {
                        Genres = genres;
                        Success = true;
                        ErrorMessage = string.Empty;
                    }
                    else {
                        Success = false;
                        ErrorMessage = "Получен пустой список жанров";
                    }
                }
                catch (JsonException ex) {
                    Success = false;
                    ErrorMessage = $"Ошибка десериализации: {ex.Message}";
                }
            }
            else {
                Success = false;
                ErrorMessage = $"Ошибка HTTP: {response.StatusCode}";
            }
        }
        catch (UnauthorizedAccessException ex) {
            Success = false;
            ErrorMessage = ex.Message;
        }
        catch (Exception ex) {
            Success = false;
            ErrorMessage = $"Ошибка при получении списка жанров: {ex.Message}";
        }

        DataLoaded?.Invoke();
    }

    public async Task GetGamesListAsync(int pageNo = 1)
    {
        try {
            var urlBuilder = new StringBuilder("games");

            if (SelectedGenre != null && !string.IsNullOrEmpty(SelectedGenre.NormalizedName)) {
                urlBuilder.Append($"/{SelectedGenre.NormalizedName}");
            }

            var queryParams = new List<string>();
            if (pageNo > 1) {
                queryParams.Add($"pageNo={pageNo}");
            }

            queryParams.Add($"pageSize={_pageSize}");

            if (queryParams.Any()) {
                urlBuilder.Append($"?{string.Join("&", queryParams)}");
            }

            var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, urlBuilder.ToString());
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode) {
                try {
                    var responseData = await response.Content
                        .ReadFromJsonAsync<ResponseData<ListModel<Game>>>(_serializerOptions);

                    if (responseData is { Successfull: true, Data: not null }) {
                        Games = responseData.Data.Items;
                        TotalPages = responseData.Data.TotalPages;
                        CurrentPage = responseData.Data.CurrentPage;
                        Success = true;
                        ErrorMessage = string.Empty;
                    }
                    else {
                        Success = false;
                        ErrorMessage = responseData?.Message ?? "Не удалось получить данные об играх";
                    }
                }
                catch (JsonException ex) {
                    Success = false;
                    ErrorMessage = $"Ошибка десериализации: {ex.Message}";
                }
            }
            else {
                Success = false;
                ErrorMessage = $"Ошибка HTTP: {response.StatusCode}";
            }
        }
        catch (UnauthorizedAccessException ex) {
            Success = false;
            ErrorMessage = ex.Message;
            Games = [];
        }
        catch (Exception ex) {
            Success = false;
            ErrorMessage = $"Ошибка при получении списка игр: {ex.Message}";
            Games = [];
        }

        DataLoaded?.Invoke();
    }

}
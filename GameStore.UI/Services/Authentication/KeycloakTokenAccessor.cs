using System.Text.Json.Nodes;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GameStore.UI.Services.Authentication;

public class KeycloakTokenAccessor : ITokenAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;
    private readonly ILogger<KeycloakTokenAccessor> _logger;

    public KeycloakTokenAccessor(
        IHttpContextAccessor httpContextAccessor,
        HttpClient httpClient,
        IOptions<KeycloakSettings> options,
        ILogger<KeycloakTokenAccessor> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || httpContext.User.Identity?.IsAuthenticated != true) return null;

        try
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении access_token из HttpContext.");
            return null;
        }
    }

    public async Task<string> GetClientAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var tokenEndpoint = $"{_settings.Host}/realms/{_settings.Realm}/protocol/openid-connect/token";

        var tokenRequestParameters = new Dictionary<string, string>
        {
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["grant_type"] = AuthConstants.ClientCredentialsGrantType
        };

        using var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
        var response = await _httpClient.PostAsync(tokenEndpoint, requestContent, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Не удалось получить сервисный токен Keycloak. Статус: {StatusCode}. Детали: {Error}", response.StatusCode, errorDetails);
            throw new ApiException(response.StatusCode, null, "Не удалось получить сервисный токен авторизации.");
        }

        var responseJson = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: cancellationToken);
        var token = responseJson?["access_token"]?.GetValue<string>();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ApiException(response.StatusCode, null, "Токен доступа Keycloak пуст.");
        }

        return token;
        
    }
}
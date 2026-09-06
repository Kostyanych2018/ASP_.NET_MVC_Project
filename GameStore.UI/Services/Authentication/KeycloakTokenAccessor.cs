using System.Globalization;
using System.Text.Json.Nodes;
using GameStore.UI.Constants;
using GameStore.UI.Exceptions;
using GameStore.UI.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace GameStore.UI.Services.Authentication;

public class KeycloakTokenAccessor : ITokenAccessor
{
    private static readonly TimeSpan RefreshSkew = TimeSpan.FromSeconds(30);

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
        if (httpContext == null || httpContext.User.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        try
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            if (!IsAccessTokenExpired(await httpContext.GetTokenAsync("expires_at")))
            {
                return accessToken;
            }

            var refreshToken = await httpContext.GetTokenAsync("refresh_token");
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Access token expired and refresh_token is missing.");
                return null;
            }

            return await RefreshUserAccessTokenAsync(httpContext, refreshToken, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve or refresh access_token.");
            return null;
        }
    }

    public async Task<string> GetClientAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var tokenRequestParameters = new Dictionary<string, string>
        {
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["grant_type"] = KeycloakConstants.ClientCredentialsGrantType
        };

        var responseJson = await RequestTokenAsync(tokenRequestParameters, cancellationToken);
        var token = responseJson["access_token"]?.GetValue<string>();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ApiException(System.Net.HttpStatusCode.Unauthorized, null, "Keycloak access token is empty.");
        }

        return token;
    }

    private async Task<string?> RefreshUserAccessTokenAsync(
        HttpContext httpContext,
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var tokenRequestParameters = new Dictionary<string, string>
        {
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["grant_type"] = KeycloakConstants.RefreshTokenGrantType,
            ["refresh_token"] = refreshToken
        };

        JsonObject responseJson;
        try
        {
            responseJson = await RequestTokenAsync(tokenRequestParameters, cancellationToken);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to refresh access token via Keycloak.");
            return null;
        }

        var newAccessToken = responseJson["access_token"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(newAccessToken))
        {
            _logger.LogWarning("Keycloak refresh response did not contain access_token.");
            return null;
        }

        var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (authenticateResult.Principal == null || authenticateResult.Properties == null)
        {
            _logger.LogWarning("Cannot persist refreshed tokens: authentication cookie is missing.");
            return newAccessToken;
        }

        var tokens = authenticateResult.Properties.GetTokens().ToDictionary(t => t.Name, t => t.Value);

        tokens["access_token"] = newAccessToken;

        var newRefreshToken = responseJson["refresh_token"]?.GetValue<string>();
        if (!string.IsNullOrWhiteSpace(newRefreshToken))
        {
            tokens["refresh_token"] = newRefreshToken;
        }

        var newIdToken = responseJson["id_token"]?.GetValue<string>();
        if (!string.IsNullOrWhiteSpace(newIdToken))
        {
            tokens["id_token"] = newIdToken;
        }

        var expiresIn = responseJson["expires_in"]?.GetValue<int>();

        if (expiresIn.HasValue)
        {
            tokens["expires_at"] = DateTimeOffset.UtcNow.AddSeconds(expiresIn.Value).ToString("o", CultureInfo.InvariantCulture);
        }

        authenticateResult.Properties.StoreTokens(
            tokens.Select(pair => new AuthenticationToken { Name = pair.Key, Value = pair.Value }));

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            authenticateResult.Principal,
            authenticateResult.Properties);

        return newAccessToken;
    }

    private async Task<JsonObject> RequestTokenAsync(
        Dictionary<string, string> tokenRequestParameters,
        CancellationToken cancellationToken)
    {
        var tokenEndpoint = $"{_settings.Host}/realms/{_settings.Realm}/protocol/openid-connect/token";

        using var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
        var response = await _httpClient.PostAsync(tokenEndpoint, requestContent, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Failed to obtain Keycloak token. Grant: {GrantType}. Status: {StatusCode}. Details: {Error}",
                tokenRequestParameters.GetValueOrDefault("grant_type"),
                response.StatusCode,
                errorDetails);
            throw new ApiException(response.StatusCode, null, "Failed to obtain authorization token.");
        }

        var responseJson = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: cancellationToken);
        if (responseJson == null)
        {
            throw new ApiException(response.StatusCode, null, "Keycloak token response is empty.");
        }

        return responseJson;
    }

    private static bool IsAccessTokenExpired(string? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(expiresAt))
        {
            return true;
        }

        if (!DateTimeOffset.TryParse(expiresAt, CultureInfo.InvariantCulture, out var expiresAtUtc))
        {
            return true;
        }

        return expiresAtUtc <= DateTimeOffset.UtcNow.Add(RefreshSkew);
    }
}

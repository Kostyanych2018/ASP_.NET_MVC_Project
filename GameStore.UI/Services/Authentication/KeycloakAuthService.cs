using System.Net.Http.Headers;
using GameStore.UI.Extensions;
using GameStore.UI.Models;
using GameStore.UI.Models.Auth;
using GameStore.UI.Settings;
using Microsoft.Extensions.Options;

namespace GameStore.UI.Services.Authentication;

public class KeycloakAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly KeycloakSettings _settings;
    private readonly ApiSettings _apiSettings;
    private readonly ILogger<KeycloakAuthService> _logger;

    public KeycloakAuthService(
        HttpClient httpClient,
        ITokenAccessor tokenAccessor,
        IOptions<ApiSettings> apiSettings,
        IOptions<KeycloakSettings> options,
        ILogger<KeycloakAuthService> logger)
    {
        _httpClient = httpClient;
        _tokenAccessor = tokenAccessor;
        _settings = options.Value;
        _apiSettings = apiSettings.Value;
        _logger = logger;
    }

    public async Task RegisterUserAsync(RegisterUserViewModel model, CancellationToken cancellationToken = default)
    {
        var clientToken = await _tokenAccessor.GetClientAccessTokenAsync(cancellationToken);
        var defaultAvatarUrl = BuildApiAssetUrl(_apiSettings.DefaultAvatar);

        var userId = await CreateKeycloakUserAsync(model, defaultAvatarUrl, clientToken, cancellationToken);
        _logger.LogInformation("User {Email} registered successfully in Keycloak.", model.Email);

        if (model.Avatar == null || model.Avatar.Length == 0)
        {
            return;
        }

        try
        {
            var avatarUrl = await UploadAvatarToApiAsync(model.Avatar, clientToken, cancellationToken);
            await UpdateKeycloakAvatarAsync(userId, model.Email, avatarUrl, clientToken, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "User {Email} was created, but avatar upload failed. Default avatar will be used.", model.Email);
        }
    }

    private async Task<string> CreateKeycloakUserAsync(
        RegisterUserViewModel model,
        string avatarUrl,
        string clientToken,
        CancellationToken cancellationToken)
    {
        var keycloakUser = new KeycloakCreateUserModel
        {
            Username = model.Email,
            Email = model.Email,
            Enabled = true,
            EmailVerified = true,
            Attributes = new Dictionary<string, string>
            {
                ["avatar"] = avatarUrl
            },
            Credentials =
            [
                new KeycloakUserCredential
                {
                    Value = model.Password
                }
            ]
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, UsersEndpoint());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);
        request.Content = JsonContent.Create(keycloakUser);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);

        var userId = response.Headers.Location?.ToString().TrimEnd('/').Split('/').LastOrDefault();
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("Keycloak did not return the created user id.");
        }

        return userId;
    }

    private async Task UpdateKeycloakAvatarAsync(
        string userId,
        string email,
        string avatarUrl,
        string clientToken,
        CancellationToken cancellationToken)
    {
        var update = new KeycloakCreateUserModel
        {
            Username = email,
            Email = email,
            Enabled = true,
            EmailVerified = true,
            Attributes = new Dictionary<string, string>
            {
                ["avatar"] = avatarUrl
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{UsersEndpoint()}/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);
        request.Content = JsonContent.Create(update);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);
    }

    private async Task<string> UploadAvatarToApiAsync(
        IFormFile avatarFile,
        string clientToken,
        CancellationToken cancellationToken)
    {
        using var formData = new MultipartFormDataContent();
        await using var fileStream = avatarFile.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(avatarFile.ContentType);
        formData.Add(fileContent, "file", avatarFile.FileName);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_apiSettings.BaseUrl.TrimEnd('/')}/api/files/avatar");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);
        request.Content = formData;

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);

        var relativePath = await response.Content.ReadAsStringAsync(cancellationToken);
        return BuildApiAssetUrl(relativePath);
    }

    private string UsersEndpoint() =>
        $"{_settings.Host}/admin/realms/{_settings.Realm}/users";

    private string BuildApiAssetUrl(string relativePath) =>
        $"{_apiSettings.BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
}

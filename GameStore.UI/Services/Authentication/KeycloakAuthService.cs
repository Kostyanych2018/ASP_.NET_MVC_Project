using System.Net.Http.Headers;
using GameStore.Application.Common.Interfaces;
using GameStore.UI.Constants;
using GameStore.UI.Extensions;
using GameStore.UI.Models;
using GameStore.UI.Models.Auth;
using GameStore.UI.Settings;
using Microsoft.Extensions.Options;

namespace GameStore.UI.Services.Authentication;

public class KeycloakAuthService: IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IConfiguration configuration;
    private readonly KeycloakSettings _settings;
    private readonly string _apiBaseUrl;
    private readonly ILogger<KeycloakAuthService> _logger;

    public KeycloakAuthService(
        HttpClient httpClient,
        ITokenAccessor tokenAccessor,
        IConfiguration configuration,
        IOptions<KeycloakSettings> options,
        ILogger<KeycloakAuthService> logger)
    {
        _httpClient = httpClient;
        _tokenAccessor = tokenAccessor;
        _settings = options.Value;
        _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")!;
        _logger = logger;
    }

    public async Task RegisterUserAsync(RegisterUserViewModel model, CancellationToken cancellationToken = default)
    {
        var avatarUrl = $"{_apiBaseUrl}/{AuthConstants.DefaultAvatarPath}";
        if (model.Avatar != null && model.Avatar.Length > 0)
        {
            avatarUrl = await UploadAvatarToApiAsync(model.Avatar, cancellationToken);
        }

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
            Credentials = new List<KeycloakUserCredential>
            {
                new KeycloakUserCredential
                {
                    Value = model.Password
                }
            }
        };

        var clientToken = await _tokenAccessor.GetClientAccessTokenAsync(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.Host}/admin/realms/{_settings.Realm}/users");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);
        request.Content = JsonContent.Create(keycloakUser);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);

        _logger.LogInformation("Пользователь {Email} успешно зарегистрирован в Keycloak.", model.Email);
    }
    
    private async Task<string> UploadAvatarToApiAsync(IFormFile avatarFile, CancellationToken cancellationToken)
    {
        using var formData = new MultipartFormDataContent();
        using var fileStream = avatarFile.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(avatarFile.ContentType);
        
        formData.Add(fileContent, "file", avatarFile.FileName);

        var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/files/avatar", formData, cancellationToken);
        await response.EnsureSuccessOrThrowApiExceptionAsync(cancellationToken);

        var relativePath = await response.Content.ReadAsStringAsync(cancellationToken);
        return $"{_apiBaseUrl}/{relativePath.TrimStart('/')}";
    }
}
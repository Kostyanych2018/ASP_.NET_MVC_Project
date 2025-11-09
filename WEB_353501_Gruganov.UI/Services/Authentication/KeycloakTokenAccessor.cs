using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using WEB_353501_Gruganov.UI.HelperClasses;

namespace WEB_353501_Gruganov.UI.Services.Authentication;

public class KeycloakTokenAccessor: ITokenAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<KeycloakData> _keycloakData;
    private readonly HttpClient _httpClient;
    
    public KeycloakTokenAccessor(IHttpContextAccessor httpContextAccessor,
        IOptions<KeycloakData> keycloakData,
        HttpClient httpClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _keycloakData = keycloakData;
        _httpClient = httpClient;
    }
    public async Task SetAuthorizationHeaderAsync(HttpClient httpClient, bool isClient)
    {
        string token = isClient
            ? await GetClientToken()
            : await GetUserToken();
        
        httpClient
            .DefaultRequestHeaders
            .Authorization =  new AuthenticationHeaderValue("bearer", token);
    }

    private async Task<string> GetUserToken()
    {
        var context = _httpContextAccessor.HttpContext!;
        var authSession = await context.AuthenticateAsync("keycloak");
        if (authSession?.Principal == null) {
            throw new AuthenticationFailureException("Пользователь неавторизован");
        }

        return (await context.GetTokenAsync("keycloak", "access_token"))!;
    }

    private async Task<string> GetClientToken()
    {
        var requestUri = $"{_keycloakData.Value.Host}/realms/{_keycloakData.Value.Realm}/protocol/openidconnect/token";
        HttpContent content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>
                ("client_id", _keycloakData.Value.ClientId),
            new KeyValuePair<string, string>
                ("grant_type", "client_credentials"),
            new KeyValuePair<string, string>
                ("client_secret", _keycloakData.Value.ClientSecret)
        ]);
        
        var response = await _httpClient.PostAsync(requestUri, content);
        if (!response.IsSuccessStatusCode) {
            throw new HttpRequestException(response.StatusCode.ToString());
        }
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonObject.Parse(responseString)["access_token"].GetValue<string>();
    }

}
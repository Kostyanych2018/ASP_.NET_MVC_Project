namespace GameStore.UI.Services.Authentication;

public interface ITokenAccessor
{
    Task SetAuthorizationHeaderAsync(HttpClient httpClient,bool isClient);
}
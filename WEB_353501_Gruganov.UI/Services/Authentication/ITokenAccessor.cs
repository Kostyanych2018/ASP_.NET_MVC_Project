namespace WEB_353501_Gruganov.UI.Services.Authentication;

public interface ITokenAccessor
{
    Task SetAuthorizationHeaderAsync(HttpClient httpClient,bool isClient);
}
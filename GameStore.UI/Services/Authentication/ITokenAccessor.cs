namespace GameStore.Application.Common.Interfaces;

public interface ITokenAccessor
{
    Task SetAuthorizationHeaderAsync(HttpClient httpClient,bool isClient);
}
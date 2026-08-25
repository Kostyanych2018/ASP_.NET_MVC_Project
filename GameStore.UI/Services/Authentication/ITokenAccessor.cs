namespace GameStore.UI.Services.Authentication;

public interface ITokenAccessor
{
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    Task<string> GetClientAccessTokenAsync(CancellationToken cancellationToken = default);
}
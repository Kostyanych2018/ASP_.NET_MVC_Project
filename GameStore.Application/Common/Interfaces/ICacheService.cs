namespace GameStore.Application.Common.Interfaces;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);
    
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);
}
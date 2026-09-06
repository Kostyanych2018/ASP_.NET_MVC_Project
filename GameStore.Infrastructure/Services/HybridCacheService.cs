using GameStore.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;

namespace GameStore.Infrastructure.Services;

public class HybridCacheService : ICacheService
{
    private readonly HybridCache _hybridCache;

    public HybridCacheService(HybridCache hybridCache)
    {
        _hybridCache = hybridCache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        return await _hybridCache.GetOrCreateAsync(
            key,
            async ct => await factory(ct),
            tags: tags,
            cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await  _hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await  _hybridCache.RemoveByTagAsync(tag, cancellationToken);
    }
}
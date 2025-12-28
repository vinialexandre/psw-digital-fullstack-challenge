using HolidaysAPI.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HolidaysAPI.Infrastructure.Cache;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        var value = await factory();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
        };

        _memoryCache.Set(key, value, cacheOptions);

        return value;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}


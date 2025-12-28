using System.Text.Json;
using HolidaysAPI.Application.Interfaces;
using HolidaysAPI.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace HolidaysAPI.Infrastructure.Cache;

public class RedisCacheService : ICacheService, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly RedisSettings _settings;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        IOptions<RedisSettings> settings,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _database = _redis.GetDatabase();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
        }

        ArgumentNullException.ThrowIfNull(factory);

        if (!_settings.Enabled)
        {
            _logger.LogWarning("Redis cache is disabled. Executing factory directly.");
            return await factory();
        }

        try
        {
            var cachedValue = await _database.StringGetAsync(key);
            
            if (cachedValue.HasValue)
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return DeserializeValue<T>(cachedValue!);
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            var value = await factory();

            if (value != null)
            {
                var expirationTime = expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes);
                await SetCacheValueAsync(key, value, expirationTime);
            }

            return value;
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error occurred while accessing key: {Key}. Falling back to factory.", key);
            return await factory();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while accessing cache for key: {Key}", key);
            throw;
        }
    }

    public void Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));
        }

        if (!_settings.Enabled)
        {
            return;
        }

        try
        {
            _database.KeyDelete(key);
            _logger.LogDebug("Cache key removed: {Key}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error occurred while removing key: {Key}", key);
        }
    }

    private async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
    {
        try
        {
            var serializedValue = SerializeValue(value);
            await _database.StringSetAsync(key, serializedValue, expiration);
            _logger.LogDebug("Cache value set for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis error occurred while setting key: {Key}", key);
        }
    }

    private string SerializeValue<T>(T value)
    {
        return JsonSerializer.Serialize(value, _jsonOptions);
    }

    private T? DeserializeValue<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value, _jsonOptions);
    }

    public void Dispose()
    {
        _redis?.Dispose();
        GC.SuppressFinalize(this);
    }
}


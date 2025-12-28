namespace HolidaysAPI.Application.Interfaces;

public interface ICacheService
{
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    void Remove(string key);
}


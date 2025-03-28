namespace Shared.RedisCache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string cacheKey);
        Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiration = null);
    }
}

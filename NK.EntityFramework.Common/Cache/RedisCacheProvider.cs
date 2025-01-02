using Microsoft.Extensions.Caching.Distributed;
using NK.EntityFramework.Common.interfaces;
using NK.EntityFramework.Common.Interfaces;
using System.Text.Json;

namespace NK.EntityFramework.Common.Cache
{
    /// <summary>
    /// Provides an implementation of the <see cref="ICacheProvider"/> interface using Redis as the caching backend.
    /// </summary>
    /// <remarks>
    /// This class relies on <see cref="IDistributedCache"/> for interacting with the Redis cache.
    /// </remarks>
    /// <param name="cache">The distributed cache instance configured for Redis.</param>
    public class RedisCacheProvider(IDistributedCache cache) : ICacheProvider
    {
        private readonly IDistributedCache _cache = cache;

        /// <summary>
        /// Retrieves an object from the Redis cache based on the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The cache key associated with the object.</param>
        /// <returns>
        /// The cached object of type <typeparamref name="T"/>, or <c>default</c> if the key is not found.
        /// </returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            return cachedData != null ? JsonSerializer.Deserialize<T>(cachedData) : default;
        }

        /// <summary>
        /// Stores an object in the Redis cache with the specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the object to store.</typeparam>
        /// <param name="key">The cache key to associate with the object.</param>
        /// <param name="value">The object to cache.</param>
        /// <param name="expiration">The expiration time for the cached object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options);
        }

        /// <summary>
        /// Removes an object from the Redis cache based on the specified key.
        /// </summary>
        /// <param name="key">The cache key associated with the object to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

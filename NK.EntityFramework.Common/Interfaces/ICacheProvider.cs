using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.interfaces
{
    /// <summary>
    /// Defines an interface for a generic cache provider to handle caching operations.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Retrieves an item from the cache by its key.
        /// </summary>
        /// <typeparam name="T">The type of the cached item.</typeparam>
        /// <param name="key">The unique key identifying the cached item.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the cached item of type <typeparamref name="T"/>, 
        /// or <c>null</c> if the key is not found in the cache.
        /// </returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Stores an item in the cache with the specified key and expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the item to store in the cache.</typeparam>
        /// <param name="key">The unique key to associate with the cached item.</param>
        /// <param name="value">The item to store in the cache.</param>
        /// <param name="expiration">The duration for which the item should remain in the cache.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Removes an item from the cache by its key.
        /// </summary>
        /// <param name="key">The unique key identifying the cached item to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(string key);
    }

}

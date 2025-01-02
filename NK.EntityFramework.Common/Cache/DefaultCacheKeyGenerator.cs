using NK.EntityFramework.Common.Interfaces;
using NK.EntityFramework.Common.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Cache
{
    /// <summary>
    /// Default implementation of the <see cref="ICacheKeyGenerator"/> interface for generating unique cache keys.
    /// </summary>
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        /// <summary>
        /// Generates a unique cache key based on the specified query builder and optional pagination parameters.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity for which the cache key is being generated.</typeparam>
        /// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
        /// <param name="pageNumber">The page number for pagination (optional).</param>
        /// <param name="pageSize">The page size for pagination (optional).</param>
        /// <returns>A unique string that represents the cache key.</returns>
        /// <remarks>
        /// The cache key is composed of the entity type name, filter conditions, sort conditions, 
        /// included navigation properties, and pagination parameters (if provided).
        /// </remarks>
        public string GenerateCacheKey<TEntity>(QueryBuilder<TEntity> queryBuilder, int pageNumber = 0, int pageSize = 0) where TEntity : class
        {
            var keyBuilder = new StringBuilder(typeof(TEntity).Name);

            if (queryBuilder.Filter != null)
            {
                keyBuilder.Append($"_Filter_{queryBuilder.Filter}");
            }

            if (queryBuilder.OrderBy != null)
            {
                keyBuilder.Append("_OrderBy");
            }

            if (queryBuilder.IncludeExpressions.Any())
            {
                keyBuilder.Append($"_Includes_{string.Join(",", queryBuilder.IncludeExpressions.Select(e => e.Expression))}");
            }

            if (pageNumber > 0 && pageSize > 0)
            {
                keyBuilder.Append($"_Page_{pageNumber}_Size_{pageSize}");
            }

            return keyBuilder.ToString();
        }
    }
}

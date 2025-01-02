using NK.EntityFramework.Common.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Interfaces
{
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// Generates a unique cache key based on the query builder and pagination parameters.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>A unique cache key as a string.</returns>
        string GenerateCacheKey<TEntity>(QueryBuilder<TEntity> queryBuilder, int pageNumber = 0, int pageSize = 0) where TEntity : class;
    }

}

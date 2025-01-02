using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Query;
using NK.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Interfaces
{
    /// <summary>
    /// Represents a generic repository interface for performing CRUD operations and advanced queries on entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    public interface IRepositoryBase<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueryBuilder{TEntity}"/> for building advanced queries.
        /// </summary>
        /// <returns>A new instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        QueryBuilder<TEntity> CreateQuery();

        /// <summary>
        /// Retrieves a single entity based on the specified query conditions.
        /// </summary>
        /// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
        /// <returns>The entity that matches the query, or null if no entity is found.</returns>
        Task<TEntity> GetAsync(QueryBuilder<TEntity> queryBuilder);

        /// <summary>
        /// Retrieves a list of entities based on the specified query conditions.
        /// </summary>
        /// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
        /// <returns>A list of entities matching the query.</returns>
        Task<IEnumerable<TEntity>> GetListAsync(QueryBuilder<TEntity> queryBuilder);

        /// <summary>
        /// Checks if any entity matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>True if any entity matches the filter; otherwise, false.</returns>
        bool GetByAny(Expression<Func<TEntity, bool>> filter = null!);

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        /// <returns>A list of all entities.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        TEntity GetById(Guid id);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="newEntity">The entity with updated values.</param>
        void Update(TEntity newEntity);

        /// <summary>
        /// Deletes an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        void Delete(Guid id);

        /// <summary>
        /// Saves all changes made to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveAsync();

        /// <summary>
        /// Retrieves a paginated list of entities from the database.
        /// </summary>
        /// <param name="filter">The filter to apply to the query.</param>
        /// <param name="orderBy">The ordering logic to apply to the query.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query.</param>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list of entities matching the query.</returns>
        Task<IPagedResult<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>> filter = null!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
            string includeProperties = "",
            int pageNumber = 1,
            int pageSize = 10);
    }
}

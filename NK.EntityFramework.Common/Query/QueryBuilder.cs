using Microsoft.EntityFrameworkCore.Query;
using NK.EntityFramework.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Query
{
    /// <summary>
    /// A query builder class for dynamically constructing query filters, sorting conditions, and including related entities for queries.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to query.</typeparam>
    public class QueryBuilder<TEntity> where TEntity : class
    {
        private Expression<Func<TEntity, bool>>? _filter;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? _orderBy;
        private readonly List<IncludeExpression<TEntity>> _includeExpressions;

        /// <summary>
        /// Gets the filter expression constructed for the query.
        /// </summary>
        public Expression<Func<TEntity, bool>>? Filter => _filter;

        /// <summary>
        /// Gets the sorting logic for the query.
        /// </summary>
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy => _orderBy;

        /// <summary>
        /// Gets the collection of include expressions for related entities.
        /// </summary>
        public IEnumerable<IncludeExpression<TEntity>> IncludeExpressions => _includeExpressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder{TEntity}"/> class.
        /// </summary>
        public QueryBuilder()
        {
            _filter = null;
            _orderBy = null;
            _includeExpressions = [];
        }

        /// <summary>
        /// Adds an AND condition to the query filter.
        /// </summary>
        /// <param name="additionalCondition">The additional condition to be combined using AND.</param>
        /// <returns>The current instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        public QueryBuilder<TEntity> And(Expression<Func<TEntity, bool>> additionalCondition)
        {
            if (_filter == null)
            {
                _filter = additionalCondition;
            }
            else
            {
                _filter = _filter.And(additionalCondition);
            }
            return this;
        }

        /// <summary>
        /// Adds an OR condition to the query filter.
        /// </summary>
        /// <param name="additionalCondition">The additional condition to be combined using OR.</param>
        /// <returns>The current instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        public QueryBuilder<TEntity> Or(Expression<Func<TEntity, bool>> additionalCondition)
        {
            if (_filter == null)
            {
                _filter = additionalCondition;
            }
            else
            {
                _filter = _filter.Or(additionalCondition);
            }
            return this;
        }

        /// <summary>
        /// Adds an OrderBy condition to the query.
        /// </summary>
        /// <typeparam name="TKey">The type of the key to order by.</typeparam>
        /// <param name="keySelector">An expression to select the key for sorting.</param>
        /// <returns>The current instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if an OrderBy condition is already set.</exception>
        public QueryBuilder<TEntity> OrderByCondition<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            if (_orderBy == null)
            {
                _orderBy = q => q.OrderBy(keySelector);
            }
            else
            {
                throw new InvalidOperationException("OrderBy is already set. Use ThenBy for additional sorting.");
            }
            return this;
        }

        /// <summary>
        /// Adds a ThenBy condition to the query for additional sorting.
        /// </summary>
        /// <typeparam name="TKey">The type of the key to order by.</typeparam>
        /// <param name="keySelector">An expression to select the key for additional sorting.</param>
        /// <returns>The current instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if OrderBy is not set before using ThenBy.</exception>
        public QueryBuilder<TEntity> ThenByCondition<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            if (_orderBy == null)
            {
                throw new InvalidOperationException("OrderBy must be set before using ThenBy.");
            }

            var previousOrderBy = _orderBy;
            _orderBy = q => previousOrderBy(q).ThenBy(keySelector);
            return this;
        }

        /// <summary>
        /// Adds an Include condition to the query for including related entities.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related entity to include.</typeparam>
        /// <param name="includeExpression">An expression representing the related entity to include.</param>
        /// <returns>The current instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        public QueryBuilder<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> includeExpression)
        {
            _includeExpressions.Add(new IncludeExpression<TEntity>
            {
                Expression = (Expression<Func<TEntity, object>>)Expression.Lambda(
                    Expression.Convert(includeExpression.Body, typeof(object)),
                    includeExpression.Parameters
                ),
                ThenIncludes = new List<Extensions.IncludeExpression>()
            });
            return this;
        }


        /// <summary>
        /// Adds a ThenInclude condition to the last Include condition for including nested related entities.
        /// </summary>
        /// <typeparam name="TPreviousProperty">The type of the previous related entity in the chain.</typeparam>
        /// <typeparam name="TProperty">The type of the nested related entity to include.</typeparam>
        /// <param name="includeExpression">An expression representing the nested related entity to include.</param>
        /// <returns>The current instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if ThenInclude is called without a prior Include.</exception>
        public QueryBuilder<TEntity> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<TPreviousProperty, TProperty>> includeExpression)
        {
            if (!_includeExpressions.Any())
            {
                throw new InvalidOperationException("Include must be called before ThenInclude.");
            }

            var lastInclude = _includeExpressions.Last();
            lastInclude.ThenIncludes.Add(new Extensions.IncludeExpression
            {
                Expression = includeExpression
            });

            return this;
        }



        /// <summary>
        /// Clears all filter, sorting, and include conditions from the query.
        /// </summary>
        public void Clear()
        {
            _filter = null;
            _orderBy = null;
            _includeExpressions.Clear();
        }
    }



}

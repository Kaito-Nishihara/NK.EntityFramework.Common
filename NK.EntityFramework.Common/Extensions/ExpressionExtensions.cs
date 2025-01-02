using System;
using System.Linq.Expressions;

namespace NK.EntityFramework.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for combining expressions dynamically.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two expressions with a logical AND operator.
        /// </summary>
        /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
        /// <param name="expr1">The first expression.</param>
        /// <param name="expr2">The second expression.</param>
        /// <returns>
        /// A new expression that represents the logical AND of the two input expressions.
        /// </returns>
        /// <remarks>
        /// This method creates a new parameter to combine the expressions, allowing for reuse in LINQ queries.
        /// </remarks>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            // Combine both conditions with an AND operation
            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// Combines two expressions with a logical OR operator.
        /// </summary>
        /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
        /// <param name="expr1">The first expression.</param>
        /// <param name="expr2">The second expression.</param>
        /// <returns>
        /// A new expression that represents the logical OR of the two input expressions.
        /// </returns>
        /// <remarks>
        /// This method creates a new parameter to combine the expressions, allowing for reuse in LINQ queries.
        /// </remarks>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            // Combine both conditions with an OR operation
            var body = Expression.OrElse(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}

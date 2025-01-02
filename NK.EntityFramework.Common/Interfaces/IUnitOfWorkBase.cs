using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Interfaces
{
    /// <summary>
    /// Represents a base interface for a unit of work pattern, managing database transactions.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    public interface IUnitOfWorkBase<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// Begins a database transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommitAsync();

        /// <summary>
        /// Rolls back the current database transaction asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RollbackAsync();

        /// <summary>
        /// Gets the database context associated with this unit of work.
        /// </summary>
        TContext Context { get; }

        /// <summary>
        /// An event that is raised when a transaction is started.
        /// </summary>
        event EventHandler TransactionStarted;

        /// <summary>
        /// An event that is raised when a transaction is committed.
        /// </summary>
        event EventHandler TransactionCommitted;

        /// <summary>
        /// An event that is raised when a transaction is rolled back.
        /// </summary>
        event EventHandler TransactionRolledBack;
    }
}

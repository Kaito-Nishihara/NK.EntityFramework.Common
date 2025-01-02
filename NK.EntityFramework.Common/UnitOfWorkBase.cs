using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Interfaces;

namespace NK.EntityFramework.Common
{
    /// <summary>
    /// トランザクションおよびデータベース操作を管理するための汎用的なUnit of Work実装を表します。
    /// </summary>
    /// <typeparam name="TContext">DbContextの型。</typeparam>
    public class UnitOfWorkBase<TContext>(TContext context) : IUnitOfWorkBase<TContext> where TContext : DbContext
    {
        private readonly TContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// 現在のDbContextインスタンスを取得します。
        /// </summary>
        public TContext Context => _context;

        /// <summary>
        /// トランザクションが開始されたときに発生します。
        /// </summary>
        public event EventHandler? TransactionStarted;

        /// <summary>
        /// トランザクションが正常にコミットされたときに発生します。
        /// </summary>
        public event EventHandler? TransactionCommitted;

        /// <summary>
        /// トランザクションがロールバックされたときに発生します。
        /// </summary>
        public event EventHandler? TransactionRolledBack;

        /// <summary>
        /// 非同期で新しいデータベーストランザクションを開始します。
        /// </summary>
        /// <exception cref="InvalidOperationException">トランザクションが既に進行中の場合にスローされます。</exception>
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("トランザクションが既に進行中です。");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            OnTransactionStarted();
        }

        /// <summary>
        /// 現在のデータベーストランザクションを非同期でコミットします。
        /// </summary>
        /// <exception cref="InvalidOperationException">コミットするトランザクションが進行中でない場合にスローされます。</exception>
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("コミットする進行中のトランザクションがありません。");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                OnTransactionCommitted();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        /// <summary>
        /// 現在のデータベーストランザクションを非同期でロールバックします。
        /// </summary>
        /// <exception cref="InvalidOperationException">ロールバックするトランザクションが進行中でない場合にスローされます。</exception>
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                try
                {
                    await _transaction.RollbackAsync();
                    OnTransactionRolledBack();
                }
                finally
                {
                    DisposeTransaction();
                }
            }
        }

        /// <summary>
        /// 現在のトランザクションを破棄し、トランザクションの状態をリセットします。
        /// </summary>
        private void DisposeTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
        }

        /// <summary>
        /// UnitOfWorkインスタンスを破棄します。これには、DbContextおよび進行中のトランザクションが含まれます。
        /// </summary>
        public void Dispose()
        {
            DisposeTransaction();
            _context.Dispose();
            GC.SuppressFinalize(this); // ファイナライザが実行されないようにします。
        }

        /// <summary>
        /// TransactionStartedイベントをトリガーします。
        /// </summary>
        protected virtual void OnTransactionStarted()
        {
            TransactionStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// TransactionCommittedイベントをトリガーします。
        /// </summary>
        protected virtual void OnTransactionCommitted()
        {
            TransactionCommitted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// TransactionRolledBackイベントをトリガーします。
        /// </summary>
        protected virtual void OnTransactionRolledBack()
        {
            TransactionRolledBack?.Invoke(this, EventArgs.Empty);
        }
    }
}

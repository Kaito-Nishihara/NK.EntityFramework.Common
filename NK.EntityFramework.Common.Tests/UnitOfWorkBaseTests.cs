using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NK.EntityFramework.Common.Tests.Models;
using Xunit;

namespace NK.EntityFramework.Common.Tests
{
    public class UnitOfWorkBaseTests
    {
        private readonly DbContextOptions<TestDbContext> _options;
        private readonly SqliteConnection _connection;
        public UnitOfWorkBaseTests()
        {
            // Set up in-memory database options
            // SQLite データベースのオプションを設定
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // 接続を開く

            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_connection) // 共有接続を使用
                .Options;

            using var context = new TestDbContext(_options);
            context.Database.EnsureCreated(); // スキーマ作成
        }

        [Fact]
        public async Task BeginTransactionAsync_ShouldStartTransaction()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            // Act
            await unitOfWork.BeginTransactionAsync();

            // Assert
            Assert.NotNull(unitOfWork.Context.Database.CurrentTransaction);
        }

        [Fact]
        public async Task CommitAsync_ShouldCommitTransaction()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            await unitOfWork.BeginTransactionAsync();

            // Act
            await unitOfWork.CommitAsync();

            // Assert
            Assert.Null(context.Database.CurrentTransaction);
        }

        [Fact]
        public async Task RollbackAsync_ShouldRollbackTransaction()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            await unitOfWork.BeginTransactionAsync();

            // Act
            await unitOfWork.RollbackAsync();

            // Assert
            Assert.Null(context.Database.CurrentTransaction);
        }

        [Fact]
        public async Task Dispose_ShouldDisposeContextAndTransaction()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            await unitOfWork.BeginTransactionAsync();

            // Act
            unitOfWork.Dispose();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => context.Database.BeginTransaction());
        }

        [Fact]
        public async Task Events_ShouldTriggerCorrectly()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            bool transactionStarted = false;
            bool transactionCommitted = false;
            bool transactionRolledBack = false;

            unitOfWork.TransactionStarted += (sender, args) => transactionStarted = true;
            unitOfWork.TransactionCommitted += (sender, args) => transactionCommitted = true;
            unitOfWork.TransactionRolledBack += (sender, args) => transactionRolledBack = true;

            // Act
            await unitOfWork.BeginTransactionAsync();
            await unitOfWork.CommitAsync();

            // Assert
            Assert.True(transactionStarted);
            Assert.True(transactionCommitted);
            Assert.False(transactionRolledBack);

            // Test rollback
            await unitOfWork.BeginTransactionAsync();
            await unitOfWork.RollbackAsync();

            Assert.True(transactionRolledBack);
        }
    }

    

}

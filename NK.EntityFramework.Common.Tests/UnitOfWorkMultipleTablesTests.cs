using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Tests
{
    public class UnitOfWorkMultipleTablesTests
    {
        private readonly DbContextOptions<TestDbContext> _options;
        private readonly SqliteConnection _connection;
        public UnitOfWorkMultipleTablesTests()
        {
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
        public async Task SaveMultipleTables_ShouldSaveToAllTables()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            var user = new User { Name = "John Doe" };
            var order = new Order { ProductName = "Laptop" };

            // Act
            await unitOfWork.BeginTransactionAsync();
            try
            {
                // Add user
                unitOfWork.Context.Users.Add(user);
                await unitOfWork.Context.SaveChangesAsync();

                // Add order associated with user
                order.UserId = user.Id; // Use the generated user ID
                unitOfWork.Context.Orders.Add(order);
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }

            // Assert
            var savedUser = context.Users.FirstOrDefault(u => u.Name == "John Doe");
            var savedOrder = context.Orders.FirstOrDefault(o => o.ProductName == "Laptop");

            Assert.NotNull(savedUser);
            Assert.NotNull(savedOrder);
            Assert.Equal(savedUser.Id, savedOrder.UserId);
        }

        [Fact]
        public async Task SaveMultipleTables_ShouldRollbackOnFailure()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            var user = new User { Name = "John Doe" };
            var order = new Order { ProductName = "Laptop" };

            // Act
            await unitOfWork.BeginTransactionAsync();
            try
            {
                // Add user
                unitOfWork.Context.Users.Add(user);
                await unitOfWork.Context.SaveChangesAsync();

                // Intentionally cause an error (e.g., set invalid UserId for order)
                order.UserId = -1; // Invalid UserId
                unitOfWork.Context.Orders.Add(order);

                // This line should throw an exception due to foreign key constraint violation
                await unitOfWork.Context.SaveChangesAsync();

                // If the code reaches here, the test should fail
                Assert.Fail("Expected an exception to be thrown, but it was not.");
            }
            catch
            {
                // Rollback should be triggered
                await unitOfWork.RollbackAsync();
            }

            // Assert
            // Verify that no data was saved to the database
            var savedUser = context.Users.FirstOrDefault(u => u.Name == "John Doe");
            var savedOrder = context.Orders.FirstOrDefault(o => o.ProductName == "Laptop");

            Assert.Null(savedUser); // User should not exist
            Assert.Null(savedOrder); // Order should not exist
        }

        [Fact]
        public async Task UpdateExistingRecord_ShouldRollbackOnFailure()
        {
            // Arrange
            var context = new TestDbContext(_options);
            var unitOfWork = new UnitOfWorkBase<TestDbContext>(context);

            // 既存レコードの準備
            var initialUser = new User { Name = "Original Name" };
            context.Users.Add(initialUser);
            await context.SaveChangesAsync(); // 初期データを保存

            // 更新時にロールバックの動作を確認する
            await unitOfWork.BeginTransactionAsync();
            try
            {
                // Act: レコードを更新
                var userToUpdate = unitOfWork.Context.Users.First(u => u.Name == "Original Name");
                userToUpdate.Name = "Updated Name";
                unitOfWork.Context.Users.Update(userToUpdate);

                // 意図的に例外を発生させる (外部キー違反など)
                var invalidOrder = new Order { ProductName = "Laptop", UserId = -1 }; // 存在しない UserId
                unitOfWork.Context.Orders.Add(invalidOrder);

                // コミット (例外発生)
                await unitOfWork.CommitAsync();

                // ここに到達した場合、テストを失敗させる
                Assert.Fail("Expected an exception to be thrown, but it was not.");
            }
            catch
            {
                // Rollback の実行
                await unitOfWork.RollbackAsync();
            }
            // ロールバック後に変更が追跡されないようにする
            context.ChangeTracker.Clear();
            // Assert: ロールバックが正しく動作しているか確認
            var userAfterRollback = context.Users.First(u => u.Id == initialUser.Id);

            Assert.Equal("Original Name", userAfterRollback.Name); // 元の名前に戻っていることを確認
        }
    }
}

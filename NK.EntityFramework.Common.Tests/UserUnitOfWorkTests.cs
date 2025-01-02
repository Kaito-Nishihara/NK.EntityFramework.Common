using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Tests.Models;
using NK.EntityFramework.Common.Tests.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Tests
{
    public class UserUnitOfWorkTests
    {
        private readonly DbContextOptions<TestDbContext> _options;
        private readonly SqliteConnection _connection;
        public UserUnitOfWorkTests() 
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
        public async Task UnitOfWork_ShouldCommitTransactionSuccessfully()
        {
            // Arrange            
            using var context = new TestDbContext(_options);
            var unitOfWork = new UserUnitOfWork(context);

            await unitOfWork.BeginTransactionAsync();

            // Act
            try
            {
                var user = new User { Name = "Bob" };
                await unitOfWork.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var order = new Order { UserId = user.Id, ProductName = "Phone" };
                await unitOfWork.Orders.AddAsync(order);
                await context.SaveChangesAsync();

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }

            // Assert
            var users = await context.Users.ToListAsync();
            var orders = await context.Orders.ToListAsync();

            Assert.Single(users);
            Assert.Single(orders);
            Assert.Equal("Bob", users.First().Name);
            Assert.Equal("Phone", orders.First().ProductName);
        }

        [Fact]
        public async Task UnitOfWork_ShouldRollbackTransactionOnError()
        {
            // Arrange            
            using var context = new TestDbContext(_options);
            var unitOfWork = new UserUnitOfWork(context);

            await unitOfWork.BeginTransactionAsync();

            // Act
            try
            {
                var user = new User { Name = "Alice" };
                await unitOfWork.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var order = new Order { UserId = 100, ProductName = "Tablet" };
                await unitOfWork.Orders.AddAsync(order);
                await context.SaveChangesAsync();

                await unitOfWork.CommitAsync();
            }
            catch
            {
                // Rollback する
                await unitOfWork.RollbackAsync();
                context.ChangeTracker.Clear();
            }

            // Assert
            var users = await context.Users.ToListAsync();
            var orders = await context.Orders.ToListAsync();

            // データが保存されていないことを確認
            Assert.Empty(users);
            Assert.Empty(orders);
        }

    }
}

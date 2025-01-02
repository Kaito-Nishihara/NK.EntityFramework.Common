using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Query;
using NK.EntityFramework.Common.Tests.Models;

namespace NK.EntityFramework.Common.Tests
{
    public class RepositoryBaseTests
    {
        private static DbContextOptions<TestDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // 一意のデータベース名を使用
                .Options;
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntityToDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new TestDbContext(options);
            var repository = new RepositoryBase<TestEntity, TestDbContext>(context);
            var newEntity = new TestEntity { Id = Guid.NewGuid(), Name = "NewEntity", IsActive = true };

            // Act
            await repository.AddAsync(newEntity);
            await repository.SaveAsync();

            // Assert
            var addedEntity = context.TestEntities.SingleOrDefault(e => e.Id == newEntity.Id);
            Assert.NotNull(addedEntity);
            Assert.Equal("NewEntity", addedEntity?.Name);
        }

        [Fact]
        public async Task Update_ShouldUpdateEntityInDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new TestDbContext(options);
            var repository = new RepositoryBase<TestEntity, TestDbContext>(context);
            var existingEntity = new TestEntity { Id = Guid.NewGuid(), Name = "OldEntity", IsActive = true };
            context.TestEntities.Add(existingEntity);
            await context.SaveChangesAsync();

            // 更新対象のプロパティを変更
            existingEntity.Name = "UpdatedEntity";

            // Act
            repository.Update(existingEntity);
            await repository.SaveAsync();

            // Assert
            var updatedEntity = context.TestEntities.SingleOrDefault(e => e.Id == existingEntity.Id);
            Assert.NotNull(updatedEntity);
            Assert.Equal("UpdatedEntity", updatedEntity?.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntityFromDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new TestDbContext(options);
            var repository = new RepositoryBase<TestEntity, TestDbContext>(context);
            var existingEntity = new TestEntity { Id = Guid.NewGuid(), Name = "ToDeleteEntity", IsActive = true };
            context.TestEntities.Add(existingEntity);
            await context.SaveChangesAsync();

            // Act
            repository.Delete(existingEntity.Id);
            await repository.SaveAsync();

            // Assert
            var deletedEntity = context.TestEntities.SingleOrDefault(e => e.Id == existingEntity.Id);
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnSingleEntity_WhenFilterMatches()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new TestDbContext(options);
            var repository = new RepositoryBase<TestEntity, TestDbContext>(context);
            context.TestEntities.AddRange(new List<TestEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "TestEntity1", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "TestEntity2", IsActive = false }
            });
            await context.SaveChangesAsync();

            var queryBuilder = new QueryBuilder<TestEntity>();
            queryBuilder.And(x => x.Name == "TestEntity1");

            // Act
            var result = await repository.GetAsync(queryBuilder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestEntity1", result?.Name);
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnAllMatchingEntities()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new TestDbContext(options);
            var repository = new RepositoryBase<TestEntity, TestDbContext>(context);
            context.TestEntities.AddRange(new List<TestEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "TestEntity1", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "TestEntity2", IsActive = false },
                new() { Id = Guid.NewGuid(), Name = "TestEntity3", IsActive = true }
            });
            await context.SaveChangesAsync();

            var queryBuilder = new QueryBuilder<TestEntity>();
            queryBuilder.And(x => x.IsActive);

            // Act
            var result = await repository.GetListAsync(queryBuilder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPagedListAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new TestDbContext(options);
            var repository = new RepositoryBase<TestEntity, TestDbContext>(context);
            context.TestEntities.AddRange(new List<TestEntity>
            {
                new() { Id = Guid.NewGuid(), Name = "TestEntity1", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "TestEntity2", IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "TestEntity3", IsActive = true }
            });
            await context.SaveChangesAsync();

            var queryBuilder = new QueryBuilder<TestEntity>();
            queryBuilder.And(x => x.IsActive);

            // Act
            var result = await repository.GetPagedListAsync(queryBuilder, pageNumber: 1, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(3, result.TotalItemCount);
        }
    }

}

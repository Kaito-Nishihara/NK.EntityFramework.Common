# プロジェクト: NK.EntityFramework.Common.Tests クラス設計書

## ファイル: QueryBuilderTests.cs

### 名前空間: NK.EntityFramework.Common.Tests

#### クラス: QueryBuilderTests

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| And_ShouldAddFilterCondition | void |  |
| Or_ShouldAddFilterCondition | void |  |
| OrderByCondition_ShouldSetOrderBy | void |  |
| ThenByCondition_ShouldThrowIfOrderByNotSet | void |  |
| Include_ShouldAddIncludeExpression | void |  |
| ThenInclude_ShouldAddNestedIncludeExpression | void |  |
| Clear_ShouldResetAllConditions | void |  |

##### メソッド詳細

###### メソッド: And_ShouldAddFilterCondition

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void And_ShouldAddFilterCondition()
{
    // Arrange
    var queryBuilder = new QueryBuilder<Test>();
    // Act
    queryBuilder.And(e => e.Id > 5);
    // Assert
    Assert.NotNull(queryBuilder.Filter);
    Assert.True(queryBuilder.Filter.ToString().Contains("e.Id > 5"));
}
```

###### メソッド: Or_ShouldAddFilterCondition

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void Or_ShouldAddFilterCondition()
{
    // Arrange
    var queryBuilder = new QueryBuilder<Test>();
    // Act
    queryBuilder.Or(e => e.Name == "Test");
    // Assert
    Assert.NotNull(queryBuilder.Filter);
    Assert.True(queryBuilder.Filter.ToString().Contains("e.Name == \"Test\""));
}
```

###### メソッド: OrderByCondition_ShouldSetOrderBy

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void OrderByCondition_ShouldSetOrderBy()
{
    // Arrange
    var queryBuilder = new QueryBuilder<TestEntity>();
    // Act
    queryBuilder.OrderByCondition(e => e.Name);
    // Assert
    Assert.NotNull(queryBuilder.OrderBy);
}
```

###### メソッド: ThenByCondition_ShouldThrowIfOrderByNotSet

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void ThenByCondition_ShouldThrowIfOrderByNotSet()
{
    // Arrange
    var queryBuilder = new QueryBuilder<TestEntity>();
    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => queryBuilder.ThenByCondition(e => e.Id));
}
```

###### メソッド: Include_ShouldAddIncludeExpression

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void Include_ShouldAddIncludeExpression()
{
    // Arrange
    var queryBuilder = new QueryBuilder<Test>();
    // Act
    queryBuilder.Include(e => e.Related);
    // Assert
    Assert.Single(queryBuilder.IncludeExpressions);
    Assert.Contains("Related", queryBuilder.IncludeExpressions.First().Expression.ToString());
}
```

###### メソッド: ThenInclude_ShouldAddNestedIncludeExpression

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void ThenInclude_ShouldAddNestedIncludeExpression()
{
    // Arrange
    var queryBuilder = new QueryBuilder<Test>();
    queryBuilder.Include(e => e.Related);
    // Act
    queryBuilder.ThenInclude<Related, Nested>(re => re.Nested);
    // Assert
    var includeExpression = queryBuilder.IncludeExpressions.First();
    Assert.Single(includeExpression.ThenIncludes);
    Assert.Contains("Nested", includeExpression.ThenIncludes.First().Expression.ToString());
}
```

###### メソッド: Clear_ShouldResetAllConditions

- **戻り値の型**: `void`

- **引数**: なし

```csharp
[Fact]
public void Clear_ShouldResetAllConditions()
{
    // Arrange
    var queryBuilder = new QueryBuilder<Test>();
    queryBuilder.And(e => e.Id > 5);
    queryBuilder.Include(e => e.Related);
    queryBuilder.OrderByCondition(e => e.Name);
    // Act
    queryBuilder.Clear();
    // Assert
    Assert.Null(queryBuilder.Filter);
    Assert.Null(queryBuilder.OrderBy);
    Assert.Empty(queryBuilder.IncludeExpressions);
}
```


#### クラス: Test

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Id | int |
| Name | string |
| Related | Related |


#### クラス: Related

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| RelatedId | int |
| Description | string |
| Nested | Nested |


#### クラス: Nested

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| NestedId | int |
| Detail | string |


## ファイル: RepositoryBaseTests.cs

### 名前空間: NK.EntityFramework.Common.Tests

#### クラス: RepositoryBaseTests

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| CreateNewContextOptions | DbContextOptions<TestDbContext> |  |
| AddAsync_ShouldAddEntityToDatabase | Task |  |
| Update_ShouldUpdateEntityInDatabase | Task |  |
| Delete_ShouldRemoveEntityFromDatabase | Task |  |
| GetAsync_ShouldReturnSingleEntity_WhenFilterMatches | Task |  |
| GetListAsync_ShouldReturnAllMatchingEntities | Task |  |
| GetPagedListAsync_ShouldReturnPagedResult | Task |  |

##### メソッド詳細

###### メソッド: CreateNewContextOptions

- **戻り値の型**: `DbContextOptions<TestDbContext>`

- **引数**: なし

```csharp
private static DbContextOptions<TestDbContext> CreateNewContextOptions()
{
    return new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // 一意のデータベース名を使用
        .Options;
}
```

###### メソッド: AddAsync_ShouldAddEntityToDatabase

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: Update_ShouldUpdateEntityInDatabase

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: Delete_ShouldRemoveEntityFromDatabase

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: GetAsync_ShouldReturnSingleEntity_WhenFilterMatches

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: GetListAsync_ShouldReturnAllMatchingEntities

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: GetPagedListAsync_ShouldReturnPagedResult

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```


## ファイル: UnitOfWorkBaseTests.cs

### 名前空間: NK.EntityFramework.Common.Tests

#### クラス: UnitOfWorkBaseTests

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| BeginTransactionAsync_ShouldStartTransaction | Task |  |
| CommitAsync_ShouldCommitTransaction | Task |  |
| RollbackAsync_ShouldRollbackTransaction | Task |  |
| Dispose_ShouldDisposeContextAndTransaction | Task |  |
| Events_ShouldTriggerCorrectly | Task |  |

##### メソッド詳細

###### メソッド: BeginTransactionAsync_ShouldStartTransaction

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: CommitAsync_ShouldCommitTransaction

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: RollbackAsync_ShouldRollbackTransaction

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: Dispose_ShouldDisposeContextAndTransaction

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: Events_ShouldTriggerCorrectly

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```


## ファイル: UnitOfWorkMultipleTablesTests.cs

### 名前空間: NK.EntityFramework.Common.Tests

#### クラス: UnitOfWorkMultipleTablesTests

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| SaveMultipleTables_ShouldSaveToAllTables | Task |  |
| SaveMultipleTables_ShouldRollbackOnFailure | Task |  |
| UpdateExistingRecord_ShouldRollbackOnFailure | Task |  |

##### メソッド詳細

###### メソッド: SaveMultipleTables_ShouldSaveToAllTables

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: SaveMultipleTables_ShouldRollbackOnFailure

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: UpdateExistingRecord_ShouldRollbackOnFailure

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```


## ファイル: UserUnitOfWorkTests.cs

### 名前空間: NK.EntityFramework.Common.Tests

#### クラス: UserUnitOfWorkTests

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| UnitOfWork_ShouldCommitTransactionSuccessfully | Task |  |
| UnitOfWork_ShouldRollbackTransactionOnError | Task |  |

##### メソッド詳細

###### メソッド: UnitOfWork_ShouldCommitTransactionSuccessfully

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: UnitOfWork_ShouldRollbackTransactionOnError

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```


## ファイル: TestDbContext.cs

### 名前空間: NK.EntityFramework.Common.Tests.Models

#### クラス: User

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Id | int |
| Name | string |
| Orders | ICollection<Order> |


#### クラス: Order

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Id | int |
| UserId | int |
| ProductName | string |
| User | User |


#### クラス: TestEntity

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Id | Guid |
| Name | string |
| IsActive | bool |


#### クラス: TestDbContext

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Users | DbSet<User> |
| Orders | DbSet<Order> |
| TestEntities | DbSet<TestEntity> |

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| OnModelCreating | void |  |

##### メソッド詳細

###### メソッド: OnModelCreating

- **戻り値の型**: `void`

- **引数**:

  - `modelBuilder`: ModelBuilder
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    // 必要に応じて Fluent API でモデルを構成
    modelBuilder.Entity<User>().HasKey(u => u.Id);
    modelBuilder.Entity<Order>().HasKey(o => o.Id);
    // Fluent API の設定例
    modelBuilder.Entity<Order>()
        .HasOne(o => o.User)
        .WithMany(u => u.Orders)
        .HasForeignKey(o => o.UserId);
}
```


## ファイル: OrderRepository.cs

### 名前空間: NK.EntityFramework.Common.Tests.Repositories

#### クラス: OrderRepository

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| GetOrdersByUserIdAsync | Task<IEnumerable<Order>> |  特定のユーザーIDに関連する注文を取得します。   |
| GetOrdersByProductNameAsync | Task<IEnumerable<Order>> |  商品名で注文を検索します。   |

##### メソッド詳細

###### メソッド: GetOrdersByUserIdAsync

- **説明**:  特定のユーザーIDに関連する注文を取得します。  

- **戻り値の型**: `Task<IEnumerable<Order>>`

- **引数**:

  - `userId`: int
```csharp
/// <summary>
/// 特定のユーザーIDに関連する注文を取得します。
/// </summary>
public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
{
    return await dbSet.Where(order => order.UserId == userId).ToListAsync();
}
```

###### メソッド: GetOrdersByProductNameAsync

- **説明**:  商品名で注文を検索します。  

- **戻り値の型**: `Task<IEnumerable<Order>>`

- **引数**:

  - `productName`: string
```csharp
/// <summary>
/// 商品名で注文を検索します。
/// </summary>
public async Task<IEnumerable<Order>> GetOrdersByProductNameAsync(string productName)
{
    return await dbSet.Where(order => order.ProductName == productName).ToListAsync();
}
```


## ファイル: UserRepository.cs

### 名前空間: NK.EntityFramework.Common.Tests.Repositories

#### クラス: UserRepository

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| GetByNameAsync | Task<User?> |  名前でユーザーを検索します。   |
| GetUsersWithOrdersAsync | Task<IEnumerable<User>> |  ユーザーとその関連する注文を取得します。   |

##### メソッド詳細

###### メソッド: GetByNameAsync

- **説明**:  名前でユーザーを検索します。  

- **戻り値の型**: `Task<User?>`

- **引数**:

  - `name`: string
```csharp
/// <summary>
/// 名前でユーザーを検索します。
/// </summary>
public async Task<User?> GetByNameAsync(string name)
{
    return await dbSet.FirstOrDefaultAsync(user => user.Name == name);
}
```

###### メソッド: GetUsersWithOrdersAsync

- **説明**:  ユーザーとその関連する注文を取得します。  

- **戻り値の型**: `Task<IEnumerable<User>>`

- **引数**: なし

```csharp
/// <summary>
/// ユーザーとその関連する注文を取得します。
/// </summary>
public async Task<IEnumerable<User>> GetUsersWithOrdersAsync()
{
    return await dbSet.Include(user => user.Orders).ToListAsync();
}
```


## ファイル: UserUnitOfWork.cs

### 名前空間: NK.EntityFramework.Common.Tests.UnitOfWork

#### クラス: UserUnitOfWork

- **概要**:  ユニットオブワークの実装。  トランザクションとリポジトリの統合を提供します。  

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Users | UserRepository |
| Orders | OrderRepository |


## ファイル: .NETCoreApp,Version=v8.0.AssemblyAttributes.cs

## ファイル: NK.EntityFramework.Common.Tests.AssemblyInfo.cs

## ファイル: NK.EntityFramework.Common.Tests.GlobalUsings.g.cs


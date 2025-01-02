# プロジェクト: NK.EntityFramework.Common クラス設計書

## ファイル: RepositoryBase.cs

### 名前空間: NK.EntityFramework.Common

#### クラス: RepositoryBase

- **概要**:  エンティティに対してCRUD操作や高度なクエリを実行するための基本的なリポジトリクラスです。  

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| CreateQuery | QueryBuilder<TEntity> |  高度なクエリを構築するための の新しいインスタンスを作成します。   |
| GetAsync | Task<TEntity?> |  指定されたクエリ条件に基づいて単一のエンティティを取得します。   |
| GetListAsync | Task<IEnumerable<TEntity>> |  指定されたクエリ条件に基づいてエンティティのリストを取得します。   |
| GetByAny | bool |  指定されたフィルタに一致するエンティティが存在するかどうかを確認します。   |
| GetAll | IEnumerable<TEntity> |  データベースからすべてのエンティティを取得します。   |
| GetById | TEntity |  一意の識別子でエンティティを取得します。   |
| AddAsync | Task<TEntity> |  新しいエンティティをデータベースに追加します。   |
| Update | void |  データベース内の既存のエンティティを更新します。   |
| Delete | void |  一意の識別子でエンティティを削除します。   |
| SaveAsync | Task<SaveResult> |  データベースに行われたすべての変更を保存します。   |
| GetPagedListAsync | Task<IPagedResult<TEntity>> |  データベースからエンティティのページングリストを取得します。   |
| ApplyInclude | IQueryable<TEntity> |  クエリにIncludeとThenIncludeを再帰的に適用します。   |
| ApplyThenInclude | IQueryable<TEntity> |  クエリにThenIncludeを再帰的に適用します。   |

##### メソッド詳細

###### メソッド: CreateQuery

- **説明**:  高度なクエリを構築するための の新しいインスタンスを作成します。  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**: なし

```csharp
/// <summary>
/// 高度なクエリを構築するための<see cref="QueryBuilder{TEntity}"/> の新しいインスタンスを作成します。
/// </summary>
/// <returns>新しい <see cref="QueryBuilder{TEntity}"/> インスタンス。</returns>
public QueryBuilder<TEntity> CreateQuery()
{
    return new QueryBuilder<TEntity>();
}
```

###### メソッド: GetAsync

- **説明**:  指定されたクエリ条件に基づいて単一のエンティティを取得します。  

- **戻り値の型**: `Task<TEntity?>`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
```csharp
/// <summary>
/// 指定されたクエリ条件に基づいて単一のエンティティを取得します。
/// </summary>
/// <param name="queryBuilder">フィルタ、インクルード、並べ替え条件を含むクエリビルダー。</param>
/// <returns>クエリに一致するエンティティ。または、一致するエンティティが見つからない場合はnull。</returns>
public virtual async Task<TEntity?> GetAsync(QueryBuilder<TEntity> queryBuilder)
{
    IQueryable<TEntity> query = dbSet;
    if (queryBuilder.Filter is not null)
    {
        query = query.Where(queryBuilder.Filter);
    }
    foreach (var includeExpression in queryBuilder.IncludeExpressions)
    {
        query = ApplyInclude(query, includeExpression);
    }
    if (queryBuilder.OrderBy != null)
    {
        query = queryBuilder.OrderBy(query);
    }
    return await query.SingleOrDefaultAsync();
}
```

###### メソッド: GetListAsync

- **説明**:  指定されたクエリ条件に基づいてエンティティのリストを取得します。  

- **戻り値の型**: `Task<IEnumerable<TEntity>>`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
```csharp
/// <summary>
/// 指定されたクエリ条件に基づいてエンティティのリストを取得します。
/// </summary>
/// <param name="queryBuilder">フィルタ、インクルード、並べ替え条件を含むクエリビルダー。</param>
/// <returns>クエリに一致するエンティティのリスト。</returns>
public virtual async Task<IEnumerable<TEntity>> GetListAsync(QueryBuilder<TEntity> queryBuilder)
{
    IQueryable<TEntity> query = dbSet;
    if (queryBuilder.Filter is not null)
    {
        query = query.Where(queryBuilder.Filter);
    }
    foreach (var includeExpression in queryBuilder.IncludeExpressions)
    {
        query = ApplyInclude(query, includeExpression);
    }
    if (queryBuilder.OrderBy != null)
    {
        query = queryBuilder.OrderBy(query);
    }
    return await query.ToListAsync();
}
```

###### メソッド: GetByAny

- **説明**:  指定されたフィルタに一致するエンティティが存在するかどうかを確認します。  

- **戻り値の型**: `bool`

- **引数**:

  - `filter`: Expression<Func<TEntity, bool>>
```csharp
/// <summary>
/// 指定されたフィルタに一致するエンティティが存在するかどうかを確認します。
/// </summary>
/// <param name="filter">適用するフィルタ。</param>
/// <returns>フィルタに一致するエンティティが存在する場合はtrue。それ以外の場合はfalse。</returns>
public virtual bool GetByAny(Expression<Func<TEntity, bool>> filter = null!)
{
    IQueryable<TEntity> query = dbSet;
    if (filter is not null)
    {
        query = query.Where(filter);
    }
    return query.Any();
}
```

###### メソッド: GetAll

- **説明**:  データベースからすべてのエンティティを取得します。  

- **戻り値の型**: `IEnumerable<TEntity>`

- **引数**: なし

```csharp
/// <summary>
/// データベースからすべてのエンティティを取得します。
/// </summary>
/// <returns>すべてのエンティティのリスト。</returns>
public virtual IEnumerable<TEntity> GetAll()
{
    return [.. dbSet];
}
```

###### メソッド: GetById

- **説明**:  一意の識別子でエンティティを取得します。  

- **戻り値の型**: `TEntity`

- **引数**:

  - `id`: Guid
```csharp
/// <summary>
/// 一意の識別子でエンティティを取得します。
/// </summary>
/// <param name="id">エンティティの一意の識別子。</param>
/// <returns>エンティティが見つかった場合はそのエンティティ。それ以外の場合はnull。</returns>
public virtual TEntity GetById(Guid id)
{
    return dbSet.Find(id)!;
}
```

###### メソッド: AddAsync

- **説明**:  新しいエンティティをデータベースに追加します。  

- **戻り値の型**: `Task<TEntity>`

- **引数**:

  - `entity`: TEntity
```csharp
/// <summary>
/// 新しいエンティティをデータベースに追加します。
/// </summary>
/// <param name="entity">追加するエンティティ。</param>
/// <returns>追加されたエンティティ。</returns>
public virtual async Task<TEntity> AddAsync(TEntity entity)
{
    await dbSet.AddAsync(entity);
    dbSet.Entry(entity).State = EntityState.Added;
    return entity;
}
```

###### メソッド: Update

- **説明**:  データベース内の既存のエンティティを更新します。  

- **戻り値の型**: `void`

- **引数**:

  - `newEntity`: TEntity
```csharp
/// <summary>
/// データベース内の既存のエンティティを更新します。
/// </summary>
/// <param name="newEntity">更新された値を持つエンティティ。</param>
public virtual void Update(TEntity newEntity)
{
    var entry = dbSet.Entry(newEntity);  
    foreach (var property in entry.Properties)
    {
        if (!Equals(property.OriginalValue, property.CurrentValue))
        {
            property.IsModified = true;
        }
    }
}
```

###### メソッド: Delete

- **説明**:  一意の識別子でエンティティを削除します。  

- **戻り値の型**: `void`

- **引数**:

  - `id`: Guid
```csharp
/// <summary>
/// 一意の識別子でエンティティを削除します。
/// </summary>
/// <param name="id">削除するエンティティの一意の識別子。</param>
/// <exception cref="InvalidOperationException">エンティティが存在しない場合にスローされます。</exception>
public virtual void Delete(Guid id)
{
    var entity = dbSet.Find(id) ?? throw new InvalidOperationException($"No entity found with ID {id}.");
    dbSet.Remove(entity);
    dbSet.Entry(entity).State = EntityState.Deleted;
}
```

###### メソッド: SaveAsync

- **説明**:  データベースに行われたすべての変更を保存します。  

- **戻り値の型**: `Task<SaveResult>`

- **引数**: なし

```csharp
/// <summary>
/// データベースに行われたすべての変更を保存します。
/// </summary>
/// <returns>操作の成功または失敗を示す <see cref="SaveResult"/>。</returns>
public virtual async Task<SaveResult> SaveAsync()
{
    var errors = new List<string>();
    try
    {
        await context.SaveChangesAsync();
        return SaveResult.Success;
    }
    catch (DbUpdateConcurrencyException ex)
    {
        errors.Add($"Concurrency conflict detected. {ex.Message}");
        return SaveResult.Failed(errors);
    }
    catch (Exception ex)
    {
        errors.Add($"An unexpected error occurred: {ex.Message}");
        return SaveResult.Failed(errors);
    }
}
```

###### メソッド: GetPagedListAsync

- **説明**:  データベースからエンティティのページングリストを取得します。  

- **戻り値の型**: `Task<IPagedResult<TEntity>>`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
  - `pageNumber`: int
  - `pageSize`: int
```csharp
/// <summary>
/// データベースからエンティティのページングリストを取得します。
/// </summary>
public virtual async Task<IPagedResult<TEntity>> GetPagedListAsync(
QueryBuilder<TEntity> queryBuilder,
 int pageNumber = 1,
 int pageSize = 10)
{
    IQueryable<TEntity> query = dbSet;
    if (queryBuilder.Filter is not null)
    {
        query = query.Where(queryBuilder.Filter);
    }
    foreach (var includeExpression in queryBuilder.IncludeExpressions)
    {
        query = ApplyInclude(query, includeExpression);
    }
    if (queryBuilder.OrderBy != null)
    {
        query = queryBuilder.OrderBy(query);
    }
    int totalCount = await query.CountAsync();
    var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    var result = new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    return result;
}
```

###### メソッド: ApplyInclude

- **説明**:  クエリにIncludeとThenIncludeを再帰的に適用します。  

- **戻り値の型**: `IQueryable<TEntity>`

- **引数**:

  - `query`: IQueryable<TEntity>
  - `includeExpression`: IncludeExpression<TEntity>
```csharp
/// <summary>
/// クエリにIncludeとThenIncludeを再帰的に適用します。
/// </summary>
/// <typeparam name="TEntity">メインエンティティの型。</typeparam>
/// <param name="query">修正するIQueryableクエリ。</param>
/// <param name="includeExpression">ナビゲーションプロパティを定義するIncludeExpression。</param>
/// <returns>IncludeとThenIncludeが適用された修正済みクエリ。</returns>
private static IQueryable<TEntity> ApplyInclude<TEntity>(
    IQueryable<TEntity> query,
    IncludeExpression<TEntity> includeExpression) where TEntity : class
{
    // Apply Include
    query = query.Include(includeExpression.Expression);
    // Apply ThenIncludes recursively
    foreach (var thenInclude in includeExpression.ThenIncludes)
    {
        query = ApplyThenInclude(query, thenInclude);
    }
    return query;
}
```

###### メソッド: ApplyThenInclude

- **説明**:  クエリにThenIncludeを再帰的に適用します。  

- **戻り値の型**: `IQueryable<TEntity>`

- **引数**:

  - `query`: IQueryable<TEntity>
  - `thenIncludeExpression`: IncludeExpression
```csharp
/// <summary>
/// クエリにThenIncludeを再帰的に適用します。
/// </summary>
/// <typeparam name="TEntity">メインエンティティの型。</typeparam>
/// <typeparam name="TPreviousProperty">前のナビゲーションプロパティの型。</typeparam>
/// <param name="query">修正するIQueryableクエリ。</param>
/// <param name="thenIncludeExpression">ネストされたナビゲーションプロパティを定義するThenIncludeExpression。</param>
/// <returns>ThenIncludeが適用された修正済みクエリ。</returns>
private static IQueryable<TEntity> ApplyThenInclude<TEntity>(
    IQueryable<TEntity> query,
    IncludeExpression thenIncludeExpression) where TEntity : class
{
    query = query.Provider.CreateQuery<TEntity>(
        Expression.Call(
            typeof(EntityFrameworkQueryableExtensions),
            nameof(EntityFrameworkQueryableExtensions.ThenInclude),
            new Type[] { typeof(TEntity), thenIncludeExpression.Expression.Type },
            query.Expression,
            Expression.Quote(thenIncludeExpression.Expression)
        )
    );
    return query;
}
```


## ファイル: UnitOfWorkBase.cs

### 名前空間: NK.EntityFramework.Common

#### クラス: UnitOfWorkBase

- **概要**:  トランザクションおよびデータベース操作を管理するための汎用的なUnit of Work実装を表します。  

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Context | TContext |

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| BeginTransactionAsync | Task |  非同期で新しいデータベーストランザクションを開始します。   |
| CommitAsync | Task |  現在のデータベーストランザクションを非同期でコミットします。   |
| RollbackAsync | Task |  現在のデータベーストランザクションを非同期でロールバックします。   |
| DisposeTransaction | void |  現在のトランザクションを破棄し、トランザクションの状態をリセットします。   |
| Dispose | void |  UnitOfWorkインスタンスを破棄します。これには、DbContextおよび進行中のトランザクションが含まれます。   |
| OnTransactionStarted | void |  TransactionStartedイベントをトリガーします。   |
| OnTransactionCommitted | void |  TransactionCommittedイベントをトリガーします。   |
| OnTransactionRolledBack | void |  TransactionRolledBackイベントをトリガーします。   |

##### メソッド詳細

###### メソッド: BeginTransactionAsync

- **説明**:  非同期で新しいデータベーストランザクションを開始します。  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: CommitAsync

- **説明**:  現在のデータベーストランザクションを非同期でコミットします。  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: RollbackAsync

- **説明**:  現在のデータベーストランザクションを非同期でロールバックします。  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
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
```

###### メソッド: DisposeTransaction

- **説明**:  現在のトランザクションを破棄し、トランザクションの状態をリセットします。  

- **戻り値の型**: `void`

- **引数**: なし

```csharp
/// <summary>
/// 現在のトランザクションを破棄し、トランザクションの状態をリセットします。
/// </summary>
private void DisposeTransaction()
{
    _transaction?.Dispose();
    _transaction = null;
}
```

###### メソッド: Dispose

- **説明**:  UnitOfWorkインスタンスを破棄します。これには、DbContextおよび進行中のトランザクションが含まれます。  

- **戻り値の型**: `void`

- **引数**: なし

```csharp
/// <summary>
/// UnitOfWorkインスタンスを破棄します。これには、DbContextおよび進行中のトランザクションが含まれます。
/// </summary>
public void Dispose()
{
    DisposeTransaction();
    _context.Dispose();
    GC.SuppressFinalize(this); // ファイナライザが実行されないようにします。
}
```

###### メソッド: OnTransactionStarted

- **説明**:  TransactionStartedイベントをトリガーします。  

- **戻り値の型**: `void`

- **引数**: なし

```csharp
/// <summary>
/// TransactionStartedイベントをトリガーします。
/// </summary>
protected virtual void OnTransactionStarted()
{
    TransactionStarted?.Invoke(this, EventArgs.Empty);
}
```

###### メソッド: OnTransactionCommitted

- **説明**:  TransactionCommittedイベントをトリガーします。  

- **戻り値の型**: `void`

- **引数**: なし

```csharp
/// <summary>
/// TransactionCommittedイベントをトリガーします。
/// </summary>
protected virtual void OnTransactionCommitted()
{
    TransactionCommitted?.Invoke(this, EventArgs.Empty);
}
```

###### メソッド: OnTransactionRolledBack

- **説明**:  TransactionRolledBackイベントをトリガーします。  

- **戻り値の型**: `void`

- **引数**: なし

```csharp
/// <summary>
/// TransactionRolledBackイベントをトリガーします。
/// </summary>
protected virtual void OnTransactionRolledBack()
{
    TransactionRolledBack?.Invoke(this, EventArgs.Empty);
}
```


## ファイル: DefaultCacheKeyGenerator.cs

### 名前空間: NK.EntityFramework.Common.Cache

#### クラス: DefaultCacheKeyGenerator

- **概要**:  Default implementation of the interface for generating unique cache keys.  

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| GenerateCacheKey | string |  Generates a unique cache key based on the specified query builder and optional pagination parameters.   |

##### メソッド詳細

###### メソッド: GenerateCacheKey

- **説明**:  Generates a unique cache key based on the specified query builder and optional pagination parameters.  

- **戻り値の型**: `string`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
  - `pageNumber`: int
  - `pageSize`: int
```csharp
/// <summary>
/// Generates a unique cache key based on the specified query builder and optional pagination parameters.
/// </summary>
/// <typeparam name="TEntity">The type of the entity for which the cache key is being generated.</typeparam>
/// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
/// <param name="pageNumber">The page number for pagination (optional).</param>
/// <param name="pageSize">The page size for pagination (optional).</param>
/// <returns>A unique string that represents the cache key.</returns>
/// <remarks>
/// The cache key is composed of the entity type name, filter conditions, sort conditions, 
/// included navigation properties, and pagination parameters (if provided).
/// </remarks>
public string GenerateCacheKey<TEntity>(QueryBuilder<TEntity> queryBuilder, int pageNumber = 0, int pageSize = 0) where TEntity : class
{
    var keyBuilder = new StringBuilder(typeof(TEntity).Name);
    if (queryBuilder.Filter != null)
    {
        keyBuilder.Append($"_Filter_{queryBuilder.Filter}");
    }
    if (queryBuilder.OrderBy != null)
    {
        keyBuilder.Append("_OrderBy");
    }
    if (queryBuilder.IncludeExpressions.Any())
    {
        keyBuilder.Append($"_Includes_{string.Join(",", queryBuilder.IncludeExpressions.Select(e => e.Expression))}");
    }
    if (pageNumber > 0 && pageSize > 0)
    {
        keyBuilder.Append($"_Page_{pageNumber}_Size_{pageSize}");
    }
    return keyBuilder.ToString();
}
```


## ファイル: RedisCacheProvider.cs

### 名前空間: NK.EntityFramework.Common.Cache

#### クラス: RedisCacheProvider

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| GetAsync | Task<T?> |  |
| SetAsync | Task |  |
| RemoveAsync | Task |  |

##### メソッド詳細

###### メソッド: GetAsync

- **戻り値の型**: `Task<T?>`

- **引数**:

  - `key`: string
```csharp
public async Task<T?> GetAsync<T>(string key)
{
    var cachedData = await _cache.GetStringAsync(key);
    return cachedData != null ? JsonSerializer.Deserialize<T>(cachedData) : default;
}
```

###### メソッド: SetAsync

- **戻り値の型**: `Task`

- **引数**:

  - `key`: string
  - `value`: T
  - `expiration`: TimeSpan
```csharp
public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
{
    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = expiration
    };
    var serializedData = JsonSerializer.Serialize(value);
    await _cache.SetStringAsync(key, serializedData, options);
}
```

###### メソッド: RemoveAsync

- **戻り値の型**: `Task`

- **引数**:

  - `key`: string
```csharp
public async Task RemoveAsync(string key)
{
    await _cache.RemoveAsync(key);
}
```


## ファイル: ExpressionExtensions.cs

### 名前空間: NK.EntityFramework.Common.Extensions

#### クラス: ExpressionExtensions

- **概要**:  Provides extension methods for combining expressions dynamically.  

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| And | Expression<Func<T, bool>> |  Combines two expressions with a logical AND operator.   |
| Or | Expression<Func<T, bool>> |  Combines two expressions with a logical OR operator.   |

##### メソッド詳細

###### メソッド: And

- **説明**:  Combines two expressions with a logical AND operator.  

- **戻り値の型**: `Expression<Func<T, bool>>`

- **引数**:

  - `expr1`: Expression<Func<T, bool>>
  - `expr2`: Expression<Func<T, bool>>
```csharp
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
```

###### メソッド: Or

- **説明**:  Combines two expressions with a logical OR operator.  

- **戻り値の型**: `Expression<Func<T, bool>>`

- **引数**:

  - `expr1`: Expression<Func<T, bool>>
  - `expr2`: Expression<Func<T, bool>>
```csharp
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
```


## ファイル: IncludeExpression.cs

### 名前空間: NK.EntityFramework.Common.Extensions

#### クラス: IncludeExpression

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Expression | LambdaExpression |
| ThenIncludes | List<IncludeExpression> |


#### クラス: IncludeExpression

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Expression | Expression<Func<TEntity, object>> |


## ファイル: ICacheKeyGenerator.cs

### 名前空間: NK.EntityFramework.Common.Interfaces

#### インターフェース: ICacheKeyGenerator

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| GenerateCacheKey | string |  Generates a unique cache key based on the query builder and pagination parameters.   |

##### メソッド詳細

###### メソッド: GenerateCacheKey

- **説明**:  Generates a unique cache key based on the query builder and pagination parameters.  

- **戻り値の型**: `string`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
  - `pageNumber`: int
  - `pageSize`: int
```csharp
/// <summary>
/// Generates a unique cache key based on the query builder and pagination parameters.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
/// <param name="pageNumber">The page number for pagination.</param>
/// <param name="pageSize">The page size for pagination.</param>
/// <returns>A unique cache key as a string.</returns>
string GenerateCacheKey<TEntity>(QueryBuilder<TEntity> queryBuilder, int pageNumber = 0, int pageSize = 0) where TEntity : class;
```


## ファイル: ICacheProvider.cs

### 名前空間: NK.EntityFramework.Common.interfaces

#### インターフェース: ICacheProvider

- **概要**:  Defines an interface for a generic cache provider to handle caching operations.  

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| GetAsync | Task<T?> |  Retrieves an item from the cache by its key.   |
| SetAsync | Task |  Stores an item in the cache with the specified key and expiration time.   |
| RemoveAsync | Task |  Removes an item from the cache by its key.   |

##### メソッド詳細

###### メソッド: GetAsync

- **説明**:  Retrieves an item from the cache by its key.  

- **戻り値の型**: `Task<T?>`

- **引数**:

  - `key`: string
```csharp
/// <summary>
/// Retrieves an item from the cache by its key.
/// </summary>
/// <typeparam name="T">The type of the cached item.</typeparam>
/// <param name="key">The unique key identifying the cached item.</param>
/// <returns>
/// A task representing the asynchronous operation. The task result contains the cached item of type <typeparamref name="T"/>, 
/// or <c>null</c> if the key is not found in the cache.
/// </returns>
Task<T?> GetAsync<T>(string key);
```

###### メソッド: SetAsync

- **説明**:  Stores an item in the cache with the specified key and expiration time.  

- **戻り値の型**: `Task`

- **引数**:

  - `key`: string
  - `value`: T
  - `expiration`: TimeSpan
```csharp
/// <summary>
/// Stores an item in the cache with the specified key and expiration time.
/// </summary>
/// <typeparam name="T">The type of the item to store in the cache.</typeparam>
/// <param name="key">The unique key to associate with the cached item.</param>
/// <param name="value">The item to store in the cache.</param>
/// <param name="expiration">The duration for which the item should remain in the cache.</param>
/// <returns>A task representing the asynchronous operation.</returns>
Task SetAsync<T>(string key, T value, TimeSpan expiration);
```

###### メソッド: RemoveAsync

- **説明**:  Removes an item from the cache by its key.  

- **戻り値の型**: `Task`

- **引数**:

  - `key`: string
```csharp
/// <summary>
/// Removes an item from the cache by its key.
/// </summary>
/// <param name="key">The unique key identifying the cached item to remove.</param>
/// <returns>A task representing the asynchronous operation.</returns>
Task RemoveAsync(string key);
```


## ファイル: IRepositoryBase.cs

### 名前空間: NK.EntityFramework.Common.Interfaces

#### インターフェース: IRepositoryBase

- **概要**:  Represents a generic repository interface for performing CRUD operations and advanced queries on entities.  

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| CreateQuery | QueryBuilder<TEntity> |  Creates a new instance of for building advanced queries.   |
| GetAsync | Task<TEntity> |  Retrieves a single entity based on the specified query conditions.   |
| GetListAsync | Task<IEnumerable<TEntity>> |  Retrieves a list of entities based on the specified query conditions.   |
| GetByAny | bool |  Checks if any entity matches the specified filter.   |
| GetAll | IEnumerable<TEntity> |  Retrieves all entities from the database.   |
| GetById | TEntity |  Retrieves an entity by its unique identifier.   |
| AddAsync | Task<TEntity> |  Adds a new entity to the database.   |
| Update | void |  Updates an existing entity in the database.   |
| Delete | void |  Deletes an entity by its unique identifier.   |
| SaveAsync | Task |  Saves all changes made to the database.   |
| GetPagedListAsync | Task<IPagedResult<TEntity>> |  Retrieves a paginated list of entities from the database.   |

##### メソッド詳細

###### メソッド: CreateQuery

- **説明**:  Creates a new instance of for building advanced queries.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**: なし

```csharp
/// <summary>
/// Creates a new instance of <see cref="QueryBuilder{TEntity}"/> for building advanced queries.
/// </summary>
/// <returns>A new instance of <see cref="QueryBuilder{TEntity}"/>.</returns>
QueryBuilder<TEntity> CreateQuery();
```

###### メソッド: GetAsync

- **説明**:  Retrieves a single entity based on the specified query conditions.  

- **戻り値の型**: `Task<TEntity>`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
```csharp
/// <summary>
/// Retrieves a single entity based on the specified query conditions.
/// </summary>
/// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
/// <returns>The entity that matches the query, or null if no entity is found.</returns>
Task<TEntity> GetAsync(QueryBuilder<TEntity> queryBuilder);
```

###### メソッド: GetListAsync

- **説明**:  Retrieves a list of entities based on the specified query conditions.  

- **戻り値の型**: `Task<IEnumerable<TEntity>>`

- **引数**:

  - `queryBuilder`: QueryBuilder<TEntity>
```csharp
/// <summary>
/// Retrieves a list of entities based on the specified query conditions.
/// </summary>
/// <param name="queryBuilder">The query builder containing filters, includes, and sort conditions.</param>
/// <returns>A list of entities matching the query.</returns>
Task<IEnumerable<TEntity>> GetListAsync(QueryBuilder<TEntity> queryBuilder);
```

###### メソッド: GetByAny

- **説明**:  Checks if any entity matches the specified filter.  

- **戻り値の型**: `bool`

- **引数**:

  - `filter`: Expression<Func<TEntity, bool>>
```csharp
/// <summary>
/// Checks if any entity matches the specified filter.
/// </summary>
/// <param name="filter">The filter to apply.</param>
/// <returns>True if any entity matches the filter; otherwise, false.</returns>
bool GetByAny(Expression<Func<TEntity, bool>> filter = null!);
```

###### メソッド: GetAll

- **説明**:  Retrieves all entities from the database.  

- **戻り値の型**: `IEnumerable<TEntity>`

- **引数**: なし

```csharp
/// <summary>
/// Retrieves all entities from the database.
/// </summary>
/// <returns>A list of all entities.</returns>
IEnumerable<TEntity> GetAll();
```

###### メソッド: GetById

- **説明**:  Retrieves an entity by its unique identifier.  

- **戻り値の型**: `TEntity`

- **引数**:

  - `id`: Guid
```csharp
/// <summary>
/// Retrieves an entity by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the entity.</param>
/// <returns>The entity if found; otherwise, null.</returns>
TEntity GetById(Guid id);
```

###### メソッド: AddAsync

- **説明**:  Adds a new entity to the database.  

- **戻り値の型**: `Task<TEntity>`

- **引数**:

  - `entity`: TEntity
```csharp
/// <summary>
/// Adds a new entity to the database.
/// </summary>
/// <param name="entity">The entity to add.</param>
/// <returns>The added entity.</returns>
Task<TEntity> AddAsync(TEntity entity);
```

###### メソッド: Update

- **説明**:  Updates an existing entity in the database.  

- **戻り値の型**: `void`

- **引数**:

  - `newEntity`: TEntity
```csharp
/// <summary>
/// Updates an existing entity in the database.
/// </summary>
/// <param name="newEntity">The entity with updated values.</param>
void Update(TEntity newEntity);
```

###### メソッド: Delete

- **説明**:  Deletes an entity by its unique identifier.  

- **戻り値の型**: `void`

- **引数**:

  - `id`: Guid
```csharp
/// <summary>
/// Deletes an entity by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the entity to delete.</param>
void Delete(Guid id);
```

###### メソッド: SaveAsync

- **説明**:  Saves all changes made to the database.  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
/// <summary>
/// Saves all changes made to the database.
/// </summary>
/// <returns>A task representing the asynchronous operation.</returns>
Task SaveAsync();
```

###### メソッド: GetPagedListAsync

- **説明**:  Retrieves a paginated list of entities from the database.  

- **戻り値の型**: `Task<IPagedResult<TEntity>>`

- **引数**:

  - `filter`: Expression<Func<TEntity, bool>>
  - `orderBy`: Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>
  - `includeProperties`: string
  - `pageNumber`: int
  - `pageSize`: int
```csharp
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
```


## ファイル: IUnitOfWorkBase.cs

### 名前空間: NK.EntityFramework.Common.Interfaces

#### インターフェース: IUnitOfWorkBase

- **概要**:  Represents a base interface for a unit of work pattern, managing database transactions.  

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Context | TContext |

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| BeginTransactionAsync | Task |  Begins a database transaction asynchronously.   |
| CommitAsync | Task |  Commits the current database transaction asynchronously.   |
| RollbackAsync | Task |  Rolls back the current database transaction asynchronously.   |

##### メソッド詳細

###### メソッド: BeginTransactionAsync

- **説明**:  Begins a database transaction asynchronously.  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
/// <summary>
/// Begins a database transaction asynchronously.
/// </summary>
/// <returns>A task representing the asynchronous operation.</returns>
Task BeginTransactionAsync();
```

###### メソッド: CommitAsync

- **説明**:  Commits the current database transaction asynchronously.  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
/// <summary>
/// Commits the current database transaction asynchronously.
/// </summary>
/// <returns>A task representing the asynchronous operation.</returns>
Task CommitAsync();
```

###### メソッド: RollbackAsync

- **説明**:  Rolls back the current database transaction asynchronously.  

- **戻り値の型**: `Task`

- **引数**: なし

```csharp
/// <summary>
/// Rolls back the current database transaction asynchronously.
/// </summary>
/// <returns>A task representing the asynchronous operation.</returns>
Task RollbackAsync();
```


## ファイル: SaveResult.cs

### 名前空間: NK.EntityFramework.Common.Models

#### クラス: SaveResult

- **概要**:  Represents the result of a save operation, indicating success or failure, and providing error details if applicable.  

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Succeeded | bool |
| Success | SaveResult |
| Errors | IEnumerable<string> |

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| ToString | string |  Returns a string representation of the save result, including error details if the operation failed.   |
| Failed | SaveResult |  Creates a representing a failed save operation,  with the specified error messages.   |
| Failed | SaveResult |  Creates a representing a failed save operation,  with the specified error messages.   |

##### メソッド詳細

###### メソッド: ToString

- **説明**:  Returns a string representation of the save result, including error details if the operation failed.  

- **戻り値の型**: `string`

- **引数**: なし

```csharp
/// <summary>
/// Returns a string representation of the save result, including error details if the operation failed.
/// </summary>
/// <returns>
/// A string indicating "Succeeded" if the operation was successful,
/// or a detailed error message if it failed.
/// </returns>
public override string ToString()
{
    if (!Succeeded)
    {
        return $"Failed: {string.Join(", ", Errors)}";
    }
    return "Succeeded";
}
```

###### メソッド: Failed

- **説明**:  Creates a representing a failed save operation,  with the specified error messages.  

- **戻り値の型**: `SaveResult`

- **引数**:

  - `errors`: string[]
```csharp
/// <summary>
/// Creates a <see cref="SaveResult"/> representing a failed save operation,
/// with the specified error messages.
/// </summary>
/// <param name="errors">An array of error messages associated with the failure.</param>
/// <returns>A new instance of <see cref="SaveResult"/> with failure details.</returns>
public static SaveResult Failed(params string[] errors)
{
    var result = new SaveResult { Succeeded = false };
    if (errors != null)
    {
        result._errors.AddRange(errors);
    }
    return result;
}
```

###### メソッド: Failed

- **説明**:  Creates a representing a failed save operation,  with the specified error messages.  

- **戻り値の型**: `SaveResult`

- **引数**:

  - `errors`: List<string>?
```csharp
/// <summary>
/// Creates a <see cref="SaveResult"/> representing a failed save operation,
/// with the specified error messages.
/// </summary>
/// <param name="errors">A list of error messages associated with the failure.</param>
/// <returns>A new instance of <see cref="SaveResult"/> with failure details.</returns>
public static SaveResult Failed(List<string>? errors)
{
    var result = new SaveResult { Succeeded = false };
    if (errors != null)
    {
        result._errors.AddRange(errors);
    }
    return result;
}
```


## ファイル: QueryBuilder.cs

### 名前空間: NK.EntityFramework.Common.Query

#### クラス: QueryBuilder

- **概要**:  A query builder class for dynamically constructing query filters, sorting conditions, and including related entities for queries.  

##### プロパティ一覧

| プロパティ名 | 型         |
|-------------|------------|
| Filter | Expression<Func<TEntity, bool>>? |
| OrderBy | Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? |
| IncludeExpressions | IEnumerable<IncludeExpression<TEntity>> |

##### メソッド一覧

| メソッド名  | 戻り値の型 | 説明                              |
|------------|------------|-----------------------------------|
| And | QueryBuilder<TEntity> |  Adds an AND condition to the query filter.   |
| Or | QueryBuilder<TEntity> |  Adds an OR condition to the query filter.   |
| OrderByCondition | QueryBuilder<TEntity> |  Adds an OrderBy condition to the query.   |
| ThenByCondition | QueryBuilder<TEntity> |  Adds a ThenBy condition to the query for additional sorting.   |
| Include | QueryBuilder<TEntity> |  Adds an Include condition to the query for including related entities.   |
| ThenInclude | QueryBuilder<TEntity> |  Adds a ThenInclude condition to the last Include condition for including nested related entities.   |
| Clear | void |  Clears all filter, sorting, and include conditions from the query.   |

##### メソッド詳細

###### メソッド: And

- **説明**:  Adds an AND condition to the query filter.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**:

  - `additionalCondition`: Expression<Func<TEntity, bool>>
```csharp
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
```

###### メソッド: Or

- **説明**:  Adds an OR condition to the query filter.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**:

  - `additionalCondition`: Expression<Func<TEntity, bool>>
```csharp
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
```

###### メソッド: OrderByCondition

- **説明**:  Adds an OrderBy condition to the query.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**:

  - `keySelector`: Expression<Func<TEntity, TKey>>
```csharp
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
```

###### メソッド: ThenByCondition

- **説明**:  Adds a ThenBy condition to the query for additional sorting.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**:

  - `keySelector`: Expression<Func<TEntity, TKey>>
```csharp
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
```

###### メソッド: Include

- **説明**:  Adds an Include condition to the query for including related entities.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**:

  - `includeExpression`: Expression<Func<TEntity, TProperty>>
```csharp
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
```

###### メソッド: ThenInclude

- **説明**:  Adds a ThenInclude condition to the last Include condition for including nested related entities.  

- **戻り値の型**: `QueryBuilder<TEntity>`

- **引数**:

  - `includeExpression`: Expression<Func<TPreviousProperty, TProperty>>
```csharp
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
```

###### メソッド: Clear

- **説明**:  Clears all filter, sorting, and include conditions from the query.  

- **戻り値の型**: `void`

- **引数**: なし

```csharp
/// <summary>
/// Clears all filter, sorting, and include conditions from the query.
/// </summary>
public void Clear()
{
    _filter = null;
    _orderBy = null;
    _includeExpressions.Clear();
}
```


## ファイル: .NETCoreApp,Version=v8.0.AssemblyAttributes.cs

## ファイル: NK.EntityFramework.Common.AssemblyInfo.cs

## ファイル: NK.EntityFramework.Common.GlobalUsings.g.cs


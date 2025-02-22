using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Query;
using NK.EntityFramework.Common.Models;
using NK.Paging;
using System.Linq.Expressions;
using NK.EntityFramework.Common.Extensions;

namespace NK.EntityFramework.Common
{
    /// <summary>
    /// エンティティに対してCRUD操作や高度なクエリを実行するための基本的なリポジトリクラスです。
    /// </summary>
    /// <typeparam name="TEntity">エンティティの型。</typeparam>
    /// <typeparam name="TContext">データベースコンテキストの型。</typeparam>
    /// <remarks>
    /// <see cref="RepositoryBase{TEntity, TContext}"/> クラスの新しいインスタンスを初期化します。
    /// </remarks>
    /// <param name="context">データベースコンテキスト。</param>
    /// <param name="cacheService">キャッシュ操作を処理するためのキャッシュサービス。</param>
    public abstract class RepositoryBase<TEntity, TContext>(TContext context) where TEntity : class where TContext : DbContext
    {
        protected readonly TContext context = context;
        protected readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

        /// <summary>
        /// 高度なクエリを構築するための<see cref="QueryBuilder{TEntity}"/> の新しいインスタンスを作成します。
        /// </summary>
        /// <returns>新しい <see cref="QueryBuilder{TEntity}"/> インスタンス。</returns>
        public QueryBuilder<TEntity> CreateQuery()
        {
            return new QueryBuilder<TEntity>();
        }

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

        /// <summary>
        /// データベースからすべてのエンティティを取得します。
        /// </summary>
        /// <returns>すべてのエンティティのリスト。</returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return [.. dbSet];
        }

        /// <summary>
        /// 一意の識別子でエンティティを取得します。
        /// </summary>
        /// <param name="id">エンティティの一意の識別子。</param>
        /// <returns>エンティティが見つかった場合はそのエンティティ。それ以外の場合はnull。</returns>
        public virtual TEntity GetById(Guid id)
        {
            return dbSet.Find(id)!;
        }

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
    }
}

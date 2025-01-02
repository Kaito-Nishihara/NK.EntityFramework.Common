namespace NK.Paging
{
    public interface IPagedResult<T>
    {
        public int TotalItemCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }

        // ページ数を計算（切り上げで取得）
        public int PageCount => (int)Math.Ceiling(TotalItemCount / (double)PageSize);

        // 前のページがあるかどうか
        public bool HasPreviousPage => PageNumber > 1;

        // 次のページがあるかどうか
        public bool HasNextPage => PageNumber < PageCount;

        // 現在のページが最初のページかどうか
        public bool IsFirstPage => PageNumber == 1;

        // 現在のページが最後のページかどうか
        public bool IsLastPage => PageNumber >= PageCount;

        // ページの最初のアイテム番号
        public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

        // ページの最後のアイテム番号
        public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalItemCount);
        public IEnumerable<T> Items { get; set; }
    }

    public class PagedResult<T> : IPagedResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalItemCount { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; }

        // ページ数を計算
        /// <summary>
        /// 
        /// </summary>
        public int PageCount => (int)Math.Ceiling(TotalItemCount / (double)PageSize);

        // 前のページがあるかどうか
        /// <summary>
        /// 
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        // 次のページがあるかどうか
        /// <summary>
        /// 
        /// </summary>
        public bool HasNextPage => PageNumber < PageCount;

        // 現在のページが最初のページかどうか
        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstPage => PageNumber == 1;

        // 現在のページが最後のページかどうか
        /// <summary>
        /// 
        /// </summary>
        public bool IsLastPage => PageNumber >= PageCount;

        // ページの最初のアイテム番号
        /// <summary>
        /// 
        /// </summary>
        public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

        // ページの最後のアイテム番号
        /// <summary>
        /// 
        /// </summary>
        public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalItemCount);

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="totalItemCount"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public PagedResult(IEnumerable<T> items, int totalItemCount, int pageNumber, int pageSize)
        {
            Items = new List<T>(items);
            TotalItemCount = totalItemCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}

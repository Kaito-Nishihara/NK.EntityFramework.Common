using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Tests.Repositories
{
    public class OrderRepository(TestDbContext context) : RepositoryBase<Order, TestDbContext>(context)
    {

        /// <summary>
        /// 特定のユーザーIDに関連する注文を取得します。
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await dbSet.Where(order => order.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// 商品名で注文を検索します。
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByProductNameAsync(string productName)
        {
            return await dbSet.Where(order => order.ProductName == productName).ToListAsync();
        }
    }
}

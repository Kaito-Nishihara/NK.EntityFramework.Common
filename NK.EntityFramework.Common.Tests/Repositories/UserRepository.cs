using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using NK.EntityFramework.Common.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Tests.Repositories
{
    public class UserRepository(TestDbContext context) : RepositoryBase<User, TestDbContext>(context)
    {

        /// <summary>
        /// 名前でユーザーを検索します。
        /// </summary>
        public async Task<User?> GetByNameAsync(string name)
        {
            return await dbSet.FirstOrDefaultAsync(user => user.Name == name);
        }

        /// <summary>
        /// ユーザーとその関連する注文を取得します。
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersWithOrdersAsync()
        {
            return await dbSet.Include(user => user.Orders).ToListAsync();
        }
    }
}

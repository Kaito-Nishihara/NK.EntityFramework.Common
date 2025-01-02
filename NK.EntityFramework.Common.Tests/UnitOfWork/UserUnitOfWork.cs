using NK.EntityFramework.Common.Tests.Models;
using NK.EntityFramework.Common.Tests.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Tests.UnitOfWork
{
    /// <summary>
    /// ユニットオブワークの実装。
    /// トランザクションとリポジトリの統合を提供します。
    /// </summary>
    public class UserUnitOfWork(TestDbContext context) : UnitOfWorkBase<TestDbContext>(context)
    {
        public UserRepository Users { get; } = new UserRepository(context);
        public OrderRepository Orders { get; } = new OrderRepository(context);
    }
}

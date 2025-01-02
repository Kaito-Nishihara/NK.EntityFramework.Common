using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Extensions
{
    public class IncludeExpression
    {
        public LambdaExpression Expression { get; set; } = null!;
        public List<IncludeExpression> ThenIncludes { get; set; } = new();
    }

    public class IncludeExpression<TEntity> : IncludeExpression
    {
        public new Expression<Func<TEntity, object>> Expression { get; set; } = null!;
    }

}

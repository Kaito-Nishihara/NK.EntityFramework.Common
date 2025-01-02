using NK.EntityFramework.Common.Query;
using NK.EntityFramework.Common.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NK.EntityFramework.Common.Tests
{
    public class QueryBuilderTests
    {
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

        [Fact]
        public void ThenByCondition_ShouldThrowIfOrderByNotSet()
        {
            // Arrange
            var queryBuilder = new QueryBuilder<TestEntity>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => queryBuilder.ThenByCondition(e => e.Id));
        }

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
    }
#nullable disable
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Related Related { get; set; }
    }

    public class Related
    {
        public int RelatedId { get; set; }
        public string Description { get; set; }
        public Nested Nested { get; set; }
    }

    public class Nested
    {
        public int NestedId { get; set; }
        public string Detail { get; set; }
    }

}

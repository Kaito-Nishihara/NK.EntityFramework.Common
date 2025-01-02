using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable
namespace NK.EntityFramework.Common.Tests.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProductName { get; set; } 

        public User User { get; set; }
    }

    public class TestEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public bool IsActive { get; set; }
    }
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<TestEntity> TestEntities { get; set; }

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
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace OrderService.DBContext
{
    public class OrderDBContext : DbContext
    {
        public OrderDBContext() : base("name = OrderCS")
        {

        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(x => x.OrderId);

            modelBuilder.Entity<Order>().Property(x => x.Comment).HasMaxLength(70);

            modelBuilder.Entity<Order>().Property(x => x.Address).HasMaxLength(70);

            modelBuilder.Entity<Order>()
            .Ignore(u => u.ArticleIds);

            modelBuilder.Entity<Order>()
            .Ignore(u => u.AmountOfArticles);
        }
    }
}

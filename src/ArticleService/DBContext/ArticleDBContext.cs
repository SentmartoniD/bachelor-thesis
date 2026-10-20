using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ArticleService.DBContext
{
    public class ArticleDBContext : DbContext
    {
        public ArticleDBContext() : base("name = ArticleCS")
        {

        }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasKey(x => x.ArticleId);

            modelBuilder.Entity<Article>().Property(x => x.Name).HasMaxLength(30);

            modelBuilder.Entity<Article>().Property(x => x.Description).HasMaxLength(70);
        }

    }
}

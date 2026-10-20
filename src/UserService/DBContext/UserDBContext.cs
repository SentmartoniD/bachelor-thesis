using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace UserService.DBContext
{
    class UserDBContext : DbContext
    {
        public UserDBContext() : base("name = UserCS")
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.UserId);

            modelBuilder.Entity<User>().Property(x => x.Username).HasMaxLength(22);
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<User>().Property(x => x.Email).HasMaxLength(70);
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

            modelBuilder.Entity<User>().Property(x => x.FirstName).HasMaxLength(20);

            modelBuilder.Entity<User>().Property(x => x.LastName).HasMaxLength(20);

            modelBuilder.Entity<User>().Property(x => x.DateOfBirth).HasMaxLength(20);

            modelBuilder.Entity<User>().Property(x => x.Address).HasMaxLength(70);

            modelBuilder.Entity<User>().Property(x => x.UserType).HasMaxLength(10);

            modelBuilder.Entity<User>().Property(x => x.Password).HasMaxLength(70);

            modelBuilder.Entity<User>().Property(x => x.Approved).HasColumnAnnotation("DefaultValue", Status.UNDEFINED);

            modelBuilder.Entity<User>().Property(x => x.Verified).HasColumnAnnotation("DefaultValue", Status.UNDEFINED);
        }
    }
}

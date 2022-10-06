using Microsoft.EntityFrameworkCore;
using Primeflix.Models;

namespace Primeflix.Data
{
    // This file is used to interact with the database

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>().HasKey(a => new
            {
                a.CelebrityId,
                a.ProductId
            });

            modelBuilder.Entity<Actor>().HasOne(p => p.Product).WithMany(a => a.ActorsMovies).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Actor>().HasOne(p => p.Celebrity).WithMany(a => a.ActorsMovies).HasForeignKey(p => p.CelebrityId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Actor> Actor { get; set; }
        public DbSet<Celebrity> Celebrity { get; set;}

    }
}

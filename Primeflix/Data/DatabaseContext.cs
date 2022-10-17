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

            modelBuilder.Entity<ProductGenre>().HasKey(pg => new
            {
                pg.ProductId,
                pg.GenreId
            });

            modelBuilder.Entity<Actor>().HasKey(a => new { a.CelebrityId, a.ProductId });
            modelBuilder.Entity<ProductGenre>().HasKey(pg => new { pg.ProductId, pg.GenreId });

            modelBuilder.Entity<Actor>().HasOne(p => p.Product).WithMany(a => a.ActorsMovies).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Actor>().HasOne(c => c.Celebrity).WithMany(a => a.ActorsMovies).HasForeignKey(c => c.CelebrityId);

            modelBuilder.Entity<ProductGenre>().HasOne(p => p.Product).WithMany(pg => pg.ProductGenre).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<ProductGenre>().HasOne(g => g.Genre).WithMany(pg => pg.ProductGenre).HasForeignKey(g => g.GenreId);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Actor> Actor { get; set; }
        public virtual DbSet<Celebrity> Celebrity { get; set;}
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<ProductGenre> ProductGenre { get; set; }

    }
}

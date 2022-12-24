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
            modelBuilder.Entity<Actor>().HasKey(a => new { a.CelebrityId, a.ProductId });
            modelBuilder.Entity<Director>().HasKey(d => new { d.CelebrityId, d.ProductId });
            modelBuilder.Entity<ProductGenre>().HasKey(pg => new { pg.ProductId, pg.GenreId });
            modelBuilder.Entity<ProductTranslation>().HasKey(pt => new { pt.ProductId, pt.LanguageId});
            modelBuilder.Entity<GenreTranslation>().HasKey(gt => new { gt.GenreId, gt.LanguageId });
            modelBuilder.Entity<CartProduct>().HasKey(cp => new { cp.CartId, cp.ProductId });
            modelBuilder.Entity<OrderDetails>().HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<Actor>().HasOne(p => p.Product).WithMany(a => a.ActorsMovies).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Actor>().HasOne(c => c.Celebrity).WithMany(a => a.ActorsMovies).HasForeignKey(c => c.CelebrityId);

            modelBuilder.Entity<Director>().HasOne(p => p.Product).WithMany(d => d.DirectorsMovies).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Director>().HasOne(c => c.Celebrity).WithMany(d => d.DirectorsMovies).HasForeignKey(c => c.CelebrityId);

            modelBuilder.Entity<ProductGenre>().HasOne(p => p.Product).WithMany(pg => pg.ProductGenre).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<ProductGenre>().HasOne(g => g.Genre).WithMany(pg => pg.ProductGenre).HasForeignKey(g => g.GenreId);

            modelBuilder.Entity<ProductTranslation>().HasOne(p => p.Product).WithMany(pt => pt.ProductsTranslations).HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<ProductTranslation>().HasOne(l => l.Language).WithMany(pt => pt.ProductsTranslations).HasForeignKey(g => g.LanguageId);

            modelBuilder.Entity<GenreTranslation>().HasOne(l => l.Language).WithMany(gt => gt.GenresTranslations).HasForeignKey(l => l.LanguageId);
            modelBuilder.Entity<GenreTranslation>().HasOne(g => g.Genre).WithMany(gt => gt.GenresTranslations).HasForeignKey(g => g.GenreId);

            modelBuilder.Entity<CartProduct>().HasOne(c => c.Cart).WithMany(cp => cp.CartProducts).HasForeignKey(c => c.CartId);
            modelBuilder.Entity<CartProduct>().HasOne(p => p.Product).WithMany(cp => cp.CartProducts).HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<OrderDetails>().HasOne(o => o.Order).WithMany(od => od.OrderDetails).HasForeignKey(o => o.OrderId);
            modelBuilder.Entity<OrderDetails>().HasOne(p => p.Product).WithMany(od => od.OrderDetails).HasForeignKey(p => p.ProductId);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Celebrity> Celebrities { get; set;}
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<ProductGenre> ProductsGenres { get; set; }
        public virtual DbSet<Format> Formats { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<ProductTranslation> ProductsTranslations { get; set; }
        public virtual DbSet<GenreTranslation> GenresTranslations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartProduct> CartProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
    }
}

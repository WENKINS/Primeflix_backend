using Microsoft.EntityFrameworkCore;
using Primeflix.Models;

namespace Primeflix.Data
{
    // This file is used to interact with the database

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DbSet<Product> Product { get; set; }

    }
}

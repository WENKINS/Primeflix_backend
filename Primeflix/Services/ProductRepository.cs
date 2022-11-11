using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class ProductRepository : IProductRepository
    {
        private DatabaseContext _databaseContext;

        public ProductRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Product GetProduct(int productId)
        {
            return _databaseContext.Products.Where(p => p.Id == productId).FirstOrDefault();
        }

        public Product GetProduct(string title)
        {
            return _databaseContext.Products.Where(p => p.Title == title).FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _databaseContext.Products.OrderBy(p => p.Title).ToList();
        }

        public bool ProductExists(int productId)
        {
            return _databaseContext.Products.Any(p => p.Id == productId);
        }

        public bool ProductExists(string title)
        {
            return _databaseContext.Products.Any(p => p.Title == title);
        }
    }
}

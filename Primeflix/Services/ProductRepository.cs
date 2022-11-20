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

        public bool CreateProduct(Product product)
        {
            _databaseContext.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _databaseContext.Remove(product);
            return Save();
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

        public bool IsDuplicate(int productId, string productTitle)
        {
            var product = _databaseContext.Products.Where(p => p.Title.Trim().ToUpper() == productTitle.Trim().ToUpper() && p.Id != productId).FirstOrDefault();
            if (product != null)
            {
                // IMPLEMENT DIRECTOR NAME COMPARISON (ADD METHOD MAYBE?)
                return true;
            }
            return false;
        }

        public bool ProductExists(int productId)
        {
            return _databaseContext.Products.Any(p => p.Id == productId);
        }

        public bool ProductExists(string title)
        {
            return _databaseContext.Products.Any(p => p.Title == title);
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

        public bool UpdateProduct(Product product)
        {
            _databaseContext.Update(product);
            return Save();
        }
    }
}

using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int productId);
        Product GetProduct(string title);
        bool ProductExists(int productId);
        bool ProductExists(string title);
        bool IsDuplicate(int productId, string productTitle);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Product product);
        bool Save();
    }
}

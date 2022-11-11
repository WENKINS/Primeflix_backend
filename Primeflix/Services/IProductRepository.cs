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
    }
}

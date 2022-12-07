using Primeflix.Models;

namespace Primeflix.Services.ProductService
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int productId);
        Product GetProduct(string title);
        bool ProductExists(int productId);
        bool ProductExists(string title);
        bool IsDuplicate(int productId, string productTitle);
        ICollection<Product> FilterResults(bool recentlyAdded, int formatId, List<int> genresId);
        bool CreateProduct(Product product, List<int> directorsId, List<int> actorsId, List<int> genresId);
        bool UpdateProduct(Product product, List<int> directorsId, List<int> actorsId, List<int> genresId);
        bool DeleteProduct(Product product);
        bool Save();
    }
}

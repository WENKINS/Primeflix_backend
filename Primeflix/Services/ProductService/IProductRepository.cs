using Primeflix.Models;

namespace Primeflix.Services.ProductService
{
    public interface IProductRepository
    {
        Task<ICollection<Product>> GetProducts();
        Task<Product> GetProduct(int productId);
        Task<Product> GetProduct(string title);
        Task<bool> ProductExists(int productId);
        Task<bool> ProductExists(string title);
        Task<bool> IsDuplicate(int productId, string productTitle);
        Task<ICollection<Product>> FilterResults(bool recentlyAdded, int formatId, List<int> genresId);
        Task<bool> CreateProduct(Product product, List<int> directorsId, List<int> actorsId, List<int> genresId);
        Task<bool> UpdateProduct(Product product, List<int> directorsId, List<int> actorsId, List<int> genresId);
        Task<bool> DeleteProduct(Product product);
        Task<bool> Save();
    }
}

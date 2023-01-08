using Primeflix.DTO;
using Primeflix.Models;

namespace Primeflix.Services.ProductService
{
    public interface IProductRepository
    {
        Task<bool> ProductExists(int productId);
        Task<bool> ProductExists(string title);
        Task<bool> IsDuplicate(ICollection<string> titles, ICollection<int> directorsId);
        Task<ICollection<Product>> GetProducts();
        Task<Product> GetProduct(int productId);
        Task<Product> GetProduct(string title);
        Task<ICollection<Product>> FilterResults(bool recentlyAdded, int formatId, List<int> genresId);
        Task<ICollection<Product>> SearchProducts(string searchText);
        Task<bool> CreateProduct(Product product, List<ProductTranslation> translations, List<int> directorsId, List<int> actorsId, List<int> genresId);
        Task<bool> UpdateProduct(Product product, List<ProductTranslation> translations, List<int> directorsId, List<int> actorsId, List<int> genresId);
        Task<bool> DeleteProduct(Product product);
        Task<bool> Save();
    }
}

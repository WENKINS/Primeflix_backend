using Primeflix.Models;

namespace Primeflix.Services
{
    //Repositories are classes or components that encapsulate the logic required to access data sources. They centralize common data access functionality,
    //providing better maintainability and decoupling the infrastructure or technology used to access databases from the domain model layer.
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

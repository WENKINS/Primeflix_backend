using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IDirectorRepository
    {
        ICollection<Celebrity> GetDirectors();
        Celebrity GetDirector(int celebrityId);
        ICollection<Celebrity> GetDirectorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfADirector(int celebrityId);
        bool DirectorExists(int directorId);
    }
}

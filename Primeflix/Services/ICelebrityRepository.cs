using Primeflix.Models;

namespace Primeflix.Services
{
    public interface ICelebrityRepository
    {
        ICollection<Celebrity> GetDirectors();
        Celebrity GetDirector(int celebrityId);
        ICollection<Celebrity> GetDirectorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfADirector(int celebrityId);
        bool DirectorExists(int celebrityId);

        ICollection<Celebrity> GetActors();
        Celebrity GetActor(int celebrityId);
        ICollection<Celebrity> GetActorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfAnActor(int celebrityId);
        bool ActorExists(int celebrityId);
        bool IsDuplicate(int celebrityId, string firstName, string lastName);
    }
}

using Primeflix.Models;

namespace Primeflix.Services.CelebrityService
{
    public interface ICelebrityRepository
    {
        bool CelebrityExists(int celebrityId);
        bool IsDuplicate(int celebrityId, string firstName, string lastName);
        ICollection<Celebrity> GetCelebrities();
        Celebrity GetCelebrity(int celebrityId);
        bool CreateCelebrity(Celebrity celebrity);
        bool UpdateCelebrity(Celebrity celebrity);
        bool DeleteCelebrity(Celebrity celebrity);
        bool Save();

        // Directors
        bool DirectorExists(int celebrityId);
        ICollection<Celebrity> GetDirectors();
        Celebrity GetDirector(int celebrityId);
        ICollection<Celebrity> GetDirectorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfADirector(int celebrityId);

        //Actors
        bool ActorExists(int celebrityId);
        ICollection<Celebrity> GetActors();
        Celebrity GetActor(int celebrityId);
        ICollection<Celebrity> GetActorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfAnActor(int celebrityId);
    }
}

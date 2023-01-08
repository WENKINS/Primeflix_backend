using Primeflix.Models;

namespace Primeflix.Services.CelebrityService
{
    public interface ICelebrityRepository
    {
        Task<bool> CelebrityExists(int celebrityId);
        Task<bool> CelebrityExists(Celebrity celebrity);
        Task<bool> IsDuplicate(string firstName, string lastName);
        Task<ICollection<Celebrity>> GetCelebrities();
        Task<Celebrity> GetCelebrity(int celebrityId);
        Task<bool> CreateCelebrity(Celebrity celebrity);
        Task<bool> UpdateCelebrity(Celebrity celebrity);
        Task<bool> DeleteCelebrity(Celebrity celebrity);
        Task<bool> Save();

        // Directors
        Task<bool> DirectorExists(int celebrityId);
        Task<ICollection<Celebrity>> GetDirectors();
        Task<Celebrity> GetDirector(int celebrityId);
        Task<ICollection<Celebrity>> GetDirectorsOfAProduct(int productId);
        Task<ICollection<Product>> GetProductsOfADirector(int celebrityId);

        //Actors
        Task<bool> ActorExists(int celebrityId);
        Task<ICollection<Celebrity>> GetActors();
        Task<Celebrity> GetActor(int celebrityId);
        Task<ICollection<Celebrity>> GetActorsOfAProduct(int productId);
        Task<ICollection<Product>> GetProductsOfAnActor(int celebrityId);
    }
}

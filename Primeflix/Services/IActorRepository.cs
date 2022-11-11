using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IActorRepository
    {
        ICollection<Celebrity> GetActors();
        Celebrity GetActor(int celebrityId);
        ICollection<Celebrity> GetActorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfAnActor(int actorId);
        bool ActorExists(int actorId);
    }
}

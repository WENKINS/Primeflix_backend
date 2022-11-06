using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IActorRepository
    {
        ICollection<Actor> GetActors();
        Actor GetActor(int actorId);
        ICollection<Actor> GetActorsOfAProduct(int productId);
        ICollection<Product> GetProductsOfAnActor(int actorId);
        bool ActorExists(int actorId);
    }
}

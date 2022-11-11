using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class ActorRepository : IActorRepository
    {
        private DatabaseContext _databaseContext;

        public ActorRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool ActorExists(int actorId)
        {
            return _databaseContext.Actors.Where(a => a.CelebrityId == actorId).Any();
        }

        public Celebrity GetActor(int celebrityId)
        {
            return _databaseContext.Actors.Where(a => a.CelebrityId == celebrityId).Select(d => d.Celebrity).FirstOrDefault();
        }

        public ICollection<Celebrity> GetActors()
        {
            var actorsIdList = _databaseContext.Actors.ToList();

            var actors = new List<Celebrity>();
            foreach (var actor in actorsIdList)
            {
                actors.Add(_databaseContext.Celebrities.Where(c => c.Id == actor.CelebrityId).FirstOrDefault());
            }

            return actors;
        }

        public ICollection<Celebrity> GetActorsOfAProduct(int productId)
        {
            return _databaseContext.Actors.Where(p => p.ProductId == productId).Select(a => a.Celebrity).ToList();
        }

        public ICollection<Product> GetProductsOfAnActor(int celebrityId)
        {
            return _databaseContext.Actors.Where(d => d.CelebrityId == celebrityId).Select(p => p.Product).ToList();
        }
    }
}

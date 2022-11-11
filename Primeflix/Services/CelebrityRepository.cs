using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class CelebrityRepository : ICelebrityRepository
    {
        private DatabaseContext _databaseContext;

        public CelebrityRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool ActorExists(int celebrityId)
        {
            return _databaseContext.Actors.Where(a => a.CelebrityId == celebrityId).Any();
        }

        public bool DirectorExists(int celebrityId)
        {
            return _databaseContext.Directors.Any(d => d.CelebrityId == celebrityId);
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

        public Celebrity GetDirector(int celebrityId)
        {
            return _databaseContext.Directors.Where(d => d.CelebrityId == celebrityId).Select(d => d.Celebrity).FirstOrDefault();
        }

        public ICollection<Celebrity> GetDirectors()
        {
            var directorsIdList = _databaseContext.Directors.ToList();

            var directors = new List<Celebrity>();
            foreach (var director in directorsIdList)
            {
                directors.Add(_databaseContext.Celebrities.Where(c => c.Id == director.CelebrityId).FirstOrDefault());
            }

            return directors;
        }

        public ICollection<Celebrity> GetDirectorsOfAProduct(int productId)
        {
            return _databaseContext.Directors.Where(p => p.ProductId == productId).Select(d => d.Celebrity).ToList();
        }

        public ICollection<Product> GetProductsOfADirector(int celebrityId)
        {
            return _databaseContext.Directors.Where(d => d.CelebrityId == celebrityId).Select(p => p.Product).ToList();
        }

        public ICollection<Product> GetProductsOfAnActor(int celebrityId)
        {
            return _databaseContext.Actors.Where(d => d.CelebrityId == celebrityId).Select(p => p.Product).ToList();
        }

        public bool IsDuplicate(int celebrityId, string firstName, string lastName)
        {
            var celebrity = _databaseContext.Celebrities.Where(c => c.FirstName.Trim().ToUpper() == firstName.Trim().ToUpper() && c.LastName.Trim().ToUpper() == lastName.Trim().ToUpper() && c.Id != celebrityId);
            return celebrity == null ? false : true;
        }
    }
}

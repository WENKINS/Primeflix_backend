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
        public bool CelebrityExists(int celebrityId)
        {
            return _databaseContext.Celebrities.Any(c => c.Id == celebrityId);
        }

        public bool IsDuplicate(int celebrityId, string firstName, string lastName)
        {
            var celebrity = _databaseContext.Celebrities.Where(c => c.FirstName.Trim().ToUpper() == firstName.Trim().ToUpper() && c.LastName.Trim().ToUpper() == lastName.Trim().ToUpper() && c.Id != celebrityId).FirstOrDefault();
            return celebrity == null ? false : true;
        }

        public Celebrity GetCelebrity(int celebrityId)
        {
            return _databaseContext.Celebrities.Where(c => c.Id == celebrityId).FirstOrDefault();
        }

        public ICollection<Celebrity> GetCelebrities()
        {
            return _databaseContext.Celebrities.OrderBy(c => c.LastName).ToList();
        }

        public bool CreateCelebrity(Celebrity celebrity)
        {
            _databaseContext.Add(celebrity);
            return Save();
        }

        public bool UpdateCelebrity(Celebrity celebrity)
        {
            _databaseContext.Update(celebrity);
            return Save();
        }

        public bool DeleteCelebrity(Celebrity celebrity)
        {
            _databaseContext.Remove(celebrity);
            return Save();
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
        public bool DirectorExists(int celebrityId)
        {
            return _databaseContext.Directors.Where(d => d.CelebrityId == celebrityId).Select(d => d.Celebrity).Any();
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

        public Celebrity GetDirector(int celebrityId)
        {
            return _databaseContext.Directors.Where(d => d.CelebrityId == celebrityId).Select(d => d.Celebrity).FirstOrDefault();
        }

        public ICollection<Celebrity> GetDirectorsOfAProduct(int productId)
        {
            return _databaseContext.Directors.Where(p => p.ProductId == productId).Select(d => d.Celebrity).ToList();
        }

        public ICollection<Product> GetProductsOfADirector(int celebrityId)
        {
            return _databaseContext.Directors.Where(d => d.CelebrityId == celebrityId).Select(p => p.Product).ToList();
        }
        public bool ActorExists(int celebrityId)
        {
            return _databaseContext.Actors.Where(a => a.CelebrityId == celebrityId).Select(d => d.Celebrity).Any();
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

        public Celebrity GetActor(int celebrityId)
        {
            return _databaseContext.Actors.Where(a => a.CelebrityId == celebrityId).Select(d => d.Celebrity).FirstOrDefault();
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

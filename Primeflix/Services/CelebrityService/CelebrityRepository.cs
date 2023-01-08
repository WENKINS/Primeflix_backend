using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.CelebrityService
{
    public class CelebrityRepository : ICelebrityRepository
    {
        private readonly DatabaseContext _databaseContext;

        public CelebrityRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<bool> CelebrityExists(int celebrityId)
        {
            return _databaseContext.Celebrities
                .Where(c => c.Id == celebrityId)
                .Any();
        }

        public async Task<bool> CelebrityExists(string firstName, string lastName)
        {
            return _databaseContext.Celebrities
                .Where(c => c.FirstName.Trim().ToUpper() == firstName.Trim().ToUpper() && c.LastName.Trim().ToUpper() == lastName.Trim().ToUpper())
                .Any();
        }

        public async Task<bool> IsDuplicate(string firstName, string lastName)
        {
            var celebrity = _databaseContext.Celebrities
                .Where(c => c.FirstName.Trim().ToUpper().Equals(firstName.Trim().ToUpper()) && c.LastName.Trim().ToUpper().Equals(lastName.Trim().ToUpper()))
                .FirstOrDefault();
            return celebrity == null ? false : true;
        }

        public async Task<ICollection<Celebrity>> GetCelebrities()
        {
            return _databaseContext.Celebrities
                .OrderBy(c => c.LastName)
                .ToList();
        }

        public async Task<Celebrity> GetCelebrity(int celebrityId)
        {
            return _databaseContext.Celebrities
                .Where(c => c.Id == celebrityId)
                .FirstOrDefault();
        }

        public async Task<bool> CreateCelebrity(Celebrity celebrity)
        {
            _databaseContext.Add(celebrity);
            return await Save();
        }

        public async Task<bool> UpdateCelebrity(Celebrity celebrity)
        {
            _databaseContext.Update(celebrity);
            return await Save();
        }

        public async Task<bool> DeleteCelebrity(Celebrity celebrity)
        {
            _databaseContext.Remove(celebrity);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

        // Directors

        public async Task<bool> DirectorExists(int celebrityId)
        {
            return _databaseContext.Directors
                .Where(d => d.CelebrityId == celebrityId)
                .Select(d => d.Celebrity)
                .Any();
        }

        public async Task<ICollection<Celebrity>> GetDirectors()
        {
            var directorsIdList = _databaseContext.Directors.ToList();

            var directors = new List<Celebrity>();

            foreach (var director in directorsIdList)
            {
                directors.Add(_databaseContext.Celebrities
                                .Where(c => c.Id == director.CelebrityId)
                                .FirstOrDefault());
            }

            return directors;
        }

        public async Task<Celebrity> GetDirector(int celebrityId)
        {
            return _databaseContext.Directors
                .Where(d => d.CelebrityId == celebrityId)
                .Select(d => d.Celebrity).FirstOrDefault();
        }

        public async Task<ICollection<Celebrity>> GetDirectorsOfAProduct(int productId)
        {
            return _databaseContext.Directors
                .Where(p => p.ProductId == productId)
                .Select(d => d.Celebrity)
                .ToList();
        }

        public async Task<ICollection<Product>> GetProductsOfADirector(int celebrityId)
        {
            return _databaseContext.Directors
                .Where(d => d.CelebrityId == celebrityId)
                .Select(p => p.Product)
                .ToList();
        }

        // Actors 

        public async Task<bool> ActorExists(int celebrityId)
        {
            return _databaseContext.Actors
                .Where(a => a.CelebrityId == celebrityId)
                .Select(d => d.Celebrity)
                .Any();
        }

        public async Task<ICollection<Celebrity>> GetActors()
        {
            var actorsIdList = _databaseContext.Actors.ToList();

            var actors = new List<Celebrity>();

            foreach (var actor in actorsIdList)
            {
                actors.Add(_databaseContext.Celebrities
                                .Where(c => c.Id == actor.CelebrityId)
                                .FirstOrDefault());
            }

            return actors;
        }

        public async Task<Celebrity> GetActor(int celebrityId)
        {
            return _databaseContext.Actors.Where(a => a.CelebrityId == celebrityId).Select(d => d.Celebrity).FirstOrDefault();
        }

        public async Task<ICollection<Celebrity>> GetActorsOfAProduct(int productId)
        {
            return _databaseContext.Actors
                .Where(p => p.ProductId == productId)
                .Select(a => a.Celebrity)
                .ToList();
        }

        public async Task<ICollection<Product>> GetProductsOfAnActor(int celebrityId)
        {
            return _databaseContext.Actors
                .Where(d => d.CelebrityId == celebrityId)
                .Select(p => p.Product)
                .ToList();
        }
    }
}

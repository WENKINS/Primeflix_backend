using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class DirectorRepository : IDirectorRepository
    {
        private DatabaseContext _databaseContext;

        public DirectorRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool DirectorExists(int directorId)
        {
            return _databaseContext.Directors.Any(d => d.CelebrityId == directorId);
        }

        public Celebrity GetDirector(int celebrityId)
        {
            return _databaseContext.Directors.Where(d => d.CelebrityId == celebrityId).Select(d => d.Celebrity).FirstOrDefault();
        }

        public ICollection<Celebrity> GetDirectors()
        {
            var directorsIdList = _databaseContext.Directors.ToList();

            var directors = new List<Celebrity>();
            foreach(var director in directorsIdList)
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
    }
}

using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class GenreRepository : IGenreRepository
    {
        private DatabaseContext _databaseContext;

        public GenreRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool GenreExists(int genreId)
        {
            return _databaseContext.Genres.Any(g => g.Id == genreId);
        }

        public Genre GetGenre(int genreId)
        {
            return _databaseContext.Genres.Where(g => g.Id == genreId).FirstOrDefault();
        }

        public ICollection<Genre> GetGenres()
        {
            return _databaseContext.Genres.OrderBy(g => g.Name).ToList();
        }

        public ICollection<Genre> GetGenresOfAProduct(int productId)
        {
            return _databaseContext.ProductsGenres.Where(p => p.ProductId == productId).Select(g => g.Genre).ToList();
        }

        public ICollection<Product> GetProductsOfAGenre(int genreId)
        {
            return _databaseContext.ProductsGenres.Where(g => g.GenreId == genreId).Select(p => p.Product).ToList();
        }

        public bool IsDuplicate(int genreId, string genreName)
        {
            var genre = _databaseContext.Genres.Where(g => g.Name.Trim().ToUpper() == genreName.Trim().ToUpper() && g.Id != genreId).FirstOrDefault();
            return genre == null ? false : true;
        }
    }
}

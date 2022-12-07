using Microsoft.EntityFrameworkCore;
using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.GenreService
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

        public ICollection<Genre> GetGenres(string languageCode)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.Language.Code == languageCode)
                .Select(g => g.Genre)
                .ToList();
        }

        public Genre GetGenre(int genreId)
        {
            return _databaseContext.Genres.Where(g => g.Id == genreId).FirstOrDefault();
        }

        public Genre GetGenre(string genreName)
        {
            return _databaseContext.Genres.Where(g => g.Name == genreName).FirstOrDefault();
        }

        public bool IsDuplicate(int genreId, string genreName)
        {
            var genre = _databaseContext.Genres.Where(g => g.Name.Trim().ToUpper() == genreName.Trim().ToUpper() && g.Id != genreId).FirstOrDefault();
            return genre == null ? false : true;
        }

        public ICollection<Genre> GetGenresOfAProduct(int productId)
        {
            return _databaseContext.ProductsGenres.Where(p => p.ProductId == productId).Select(g => g.Genre).ToList();
        }

        public ICollection<Product> GetProductsOfAGenre(int genreId)
        {
            return _databaseContext.ProductsGenres.Where(g => g.GenreId == genreId).Select(p => p.Product).ToList();
        }

        public bool CreateGenre(Genre genre)
        {
            _databaseContext.Add(genre);
            return Save();
        }

        public bool UpdateGenre(Genre genre)
        {
            _databaseContext.Update(genre);
            return Save();
        }

        public bool DeleteGenre(Genre genre)
        {
            _databaseContext.Remove(genre);
            return Save();
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}

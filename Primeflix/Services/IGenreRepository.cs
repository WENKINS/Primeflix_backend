using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IGenreRepository
    {
        ICollection<Genre> GetGenres();
        Genre GetGenre(int genreId);
        Genre GetGenre(string genreName);
        ICollection<Genre> GetGenresOfAProduct(int productId);
        ICollection<Product> GetProductsOfAGenre(int genreId);
        bool GenreExists(int genreId);
        bool IsDuplicate(int genreId, string genreName);

        bool CreateGenre(Genre genre);
        bool UpdateGenre(Genre genre);
        bool DeleteGenre(Genre genre);
        bool Save();

    }
}

using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IGenreRepository
    {
        ICollection<Genre> GetGenres();
        Genre GetGenre(int genreId);
        ICollection<Genre> GetGenresOfAProduct(int productId);
        ICollection<Product> GetProductsOfAGenre(int genreId);
        bool GenreExists(int genreId);

    }
}

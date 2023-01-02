using Primeflix.Models;

namespace Primeflix.Services.GenreService
{
    public interface IGenreRepository
    {
        Task<ICollection<Genre>> GetGenres(string languageCode);
        Task<Genre> GetGenre(int genreId);
        Task<Genre> GetGenre(string genreName);
        Task<ICollection<Genre>> GetGenresOfAProduct(int productId);
        Task<ICollection<Product>> GetProductsOfAGenre(int genreId);
        Task<bool> GenreExists(int genreId);
        Task<bool> GenreExists(Genre genre);
        Task<bool> GenreExists(string genreName);
        Task<bool> IsDuplicate(int genreId, string genreName);

        Task<bool> CreateGenre(Genre genre);
        Task<bool> UpdateGenre(Genre genre);
        Task<bool> DeleteGenre(Genre genre);
        Task<bool> Save();

    }
}

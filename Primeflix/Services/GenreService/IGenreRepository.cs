using Primeflix.DTO;
using Primeflix.Models;

namespace Primeflix.Services.GenreService
{
    public interface IGenreRepository
    {
        Task<bool> GenreExists(int genreId);
        Task<bool> GenreExists(string genreName);
        Task<bool> IsDuplicate(string genreName);
        Task<ICollection<Genre>> GetGenres(string languageCode);
        Task<Genre> GetGenre(int genreId);
        Task<Genre> GetGenre(string genreName);
        Task<ICollection<Genre>> GetGenresOfAProduct(int productId);
        Task<ICollection<Product>> GetProductsOfAGenre(int genreId);
        Task<bool> CreateGenre(NewGenreDto genre);
        Task<bool> UpdateGenre(NewGenreDto genre);
        Task<bool> DeleteGenre(Genre genre);
        Task<bool> Save();

    }
}

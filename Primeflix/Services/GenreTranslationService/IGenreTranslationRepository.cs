using Primeflix.Models;

namespace Primeflix.Services.GenreTranslationService
{
    public interface IGenreTranslationRepository
    {
        Task<ICollection<GenreTranslation>> GetGenresTranslations();
        Task<GenreTranslation> GetGenreTranslation(int genreId, string languageCode);
        Task<ICollection<GenreTranslation>> GetTranslationsOfAGenre(int genreId);
        Task<ICollection<GenreTranslation>> GetGenresOfALanguage(int languageId);
        Task<bool> GenreTranslationExists(int genreId, int languageId);
        Task<bool> IsDuplicate(int genreId, int languageId);
        Task<bool> CreateGenreTranslation(GenreTranslation genreTranslation);
        Task<bool> UpdateGenreTranslation(GenreTranslation genreTranslation);
        Task<bool> DeleteGenreTranslation(GenreTranslation genreTranslation);
        Task<bool> Save();
    }
}

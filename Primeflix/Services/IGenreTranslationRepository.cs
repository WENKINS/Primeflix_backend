using Primeflix.Models;

namespace Primeflix.Services
{
    public interface IGenreTranslationRepository
    {
        ICollection<GenreTranslation> GetGenresTranslations();
        GenreTranslation GetGenreTranslation(int genreId, string languageCode);
        ICollection<GenreTranslation> GetTranslationsOfAGenre(int genreId);
        ICollection<GenreTranslation> GetGenresOfALanguage(int languageId);
        bool GenreTranslationExists(int genreId, int languageId);
        bool IsDuplicate(int genreId, int languageId);
        bool CreateGenreTranslation(GenreTranslation genreTranslation);
        bool UpdateGenreTranslation(GenreTranslation genreTranslation);
        bool DeleteGenreTranslation(GenreTranslation genreTranslation);
        bool Save();
    }
}

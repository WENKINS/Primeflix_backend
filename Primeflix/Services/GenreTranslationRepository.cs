using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services
{
    public class GenreTranslationRepository : IGenreTranslationRepository
    {
        private DatabaseContext _databaseContext;

        public GenreTranslationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool GenreTranslationExists(int genreId, int languageId)
        {
            return _databaseContext.GenresTranslations.Where(gt => gt.GenreId == genreId && gt.LanguageId == languageId).Any();
        }

        public ICollection<GenreTranslation> GetGenresTranslations()
        {
            return _databaseContext.GenresTranslations.ToList();
        }

        public GenreTranslation GetGenreTranslation(int genreId, int languageId)
        {
            return _databaseContext.GenresTranslations.Where(gt => gt.GenreId == genreId && gt.LanguageId == languageId).FirstOrDefault();
        }

        public bool IsDuplicate(int genreId, int languageId)
        {
            var genreTranslation = _databaseContext.GenresTranslations.Where(gt => gt.GenreId == genreId && gt.LanguageId == languageId).FirstOrDefault();
            return genreTranslation == null ? false : true;
        }

        public ICollection<GenreTranslation> GetTranslationsOfAGenre(int genreId)
        {
            return _databaseContext.GenresTranslations.Where(gt => gt.GenreId == genreId).ToList();
        }

        public ICollection<GenreTranslation> GetGenresOfALanguage(int languageId)
        {
            return _databaseContext.GenresTranslations.Where(gt => gt.LanguageId == languageId).ToList();
        }

        public bool CreateGenreTranslation(GenreTranslation genreTranslation)
        {
            _databaseContext.Add(genreTranslation);
            return Save();
        }

        public bool UpdateGenreTranslation(GenreTranslation genreTranslation)
        {
            _databaseContext.Update(genreTranslation);
            return Save();
        }

        public bool DeleteGenreTranslation(GenreTranslation genreTranslation)
        {
            _databaseContext.Remove(genreTranslation);
            return Save();
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }

    }
}

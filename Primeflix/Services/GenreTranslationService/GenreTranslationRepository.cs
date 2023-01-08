using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.GenreTranslationService
{
    public class GenreTranslationRepository : IGenreTranslationRepository
    {
        private readonly DatabaseContext _databaseContext;

        public GenreTranslationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> GenreTranslationExists(int genreId, int languageId)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.GenreId == genreId && gt.LanguageId == languageId)
                .Any();
        }

        public async Task<bool> IsDuplicate(int genreId, int languageId)
        {
            var genreTranslation = _databaseContext.GenresTranslations
                .Where(gt => gt.GenreId == genreId && gt.LanguageId == languageId)
                .FirstOrDefault();
            return genreTranslation == null ? false : true;
        }

        public async Task<ICollection<GenreTranslation>> GetGenresTranslations()
        {
            return _databaseContext.GenresTranslations
                .ToList();
        }

        public async Task<GenreTranslation> GetGenreTranslation(int genreId, string languageCode)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.GenreId == genreId && gt.Language.Code == languageCode)
                .FirstOrDefault();
        }


        public async Task<ICollection<GenreTranslation>> GetTranslationsOfAGenre(int genreId)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.GenreId == genreId)
                .ToList();
        }

        public async Task<ICollection<GenreTranslation>> GetGenresOfALanguage(int languageId)
        {
            return _databaseContext.GenresTranslations
                .Where(gt => gt.LanguageId == languageId)
                .ToList();
        }

        public async Task<bool> CreateGenreTranslation(GenreTranslation genreTranslation)
        {
            _databaseContext.Add(genreTranslation);
            return await Save();
        }

        public async Task<bool> UpdateGenreTranslation(GenreTranslation genreTranslation)
        {
            _databaseContext.Update(genreTranslation);
            return await Save();
        }

        public async Task<bool> DeleteGenreTranslation(GenreTranslation genreTranslation)
        {
            _databaseContext.Remove(genreTranslation);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}

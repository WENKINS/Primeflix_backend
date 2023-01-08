using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.LanguageService
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly DatabaseContext _databaseContext;

        public LanguageRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> LanguageExists(int languageId)
        {
            return _databaseContext.Languages.Any(l => l.Id == languageId);
        }

        public async Task<bool> LanguageExists(string languageCode)
        {
            return _databaseContext.Languages.Any(l => l.Code.Equals(languageCode));
        }

        public async Task<bool> IsDuplicate(int languageId, string languageName)
        {
            var language = _databaseContext.Languages.Where(l => l.Name.Trim().ToUpper() == languageName.Trim().ToUpper() && l.Id != languageId).FirstOrDefault();
            return language == null ? false : true;
        }

        public async Task<ICollection<Language>> GetLanguages()
        {
            return _databaseContext.Languages.OrderBy(l => l.Name).ToList();
        }

        public async Task<Language> GetLanguage(int languageId)
        {
            return _databaseContext.Languages.Where(l => l.Id == languageId).FirstOrDefault();
        }

        public async Task<Language> GetLanguage(string languageCode)
        {
            return _databaseContext.Languages.Where(l => l.Code == languageCode).FirstOrDefault();
        }

        public async Task<Language> GetLanguageOfAUser(int userId)
        {
            return _databaseContext.Users.Where(u => u.Id == userId).Select(u => u.Language).FirstOrDefault();
        }

        public async Task<bool> CreateLanguage(Language language)
        {
            _databaseContext.Add(language);
            return await Save();
        }

        public async Task<bool> UpdateLanguage(Language language)
        {
            _databaseContext.Update(language);
            return await Save();
        }

        public async Task<bool> DeleteLanguage(Language language)
        {
            _databaseContext.Remove(language);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}

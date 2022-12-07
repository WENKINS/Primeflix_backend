using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.LanguageService
{
    public class LanguageRepository : ILanguageRepository
    {
        private DatabaseContext _databaseContext;

        public LanguageRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool LanguageExists(int languageId)
        {
            return _databaseContext.Languages.Any(l => l.Id == languageId);
        }

        public ICollection<Language> GetLanguages()
        {
            return _databaseContext.Languages.OrderBy(l => l.Name).ToList();
        }

        public Language GetLanguage(int languageId)
        {
            return _databaseContext.Languages.Where(l => l.Id == languageId).FirstOrDefault();
        }

        public Language GetLanguage(string languageCode)
        {
            return _databaseContext.Languages.Where(l => l.Code == languageCode).FirstOrDefault();
        }

        public bool IsDuplicate(int languageId, string languageName)
        {
            var language = _databaseContext.Languages.Where(l => l.Name.Trim().ToUpper() == languageName.Trim().ToUpper() && l.Id != languageId).FirstOrDefault();
            return language == null ? false : true;
        }

        public bool CreateLanguage(Language language)
        {
            _databaseContext.Add(language);
            return Save();
        }

        public bool UpdateLanguage(Language language)
        {
            _databaseContext.Update(language);
            return Save();
        }

        public bool DeleteLanguage(Language language)
        {
            _databaseContext.Remove(language);
            return Save();
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}

using Primeflix.Models;

namespace Primeflix.Services
{
    public interface ILanguageRepository
    {
        ICollection<Language> GetLanguages();
        Language GetLanguage(int languageId);
        Language GetLanguage(string languageName);
        bool LanguageExists(int languageId);
        bool IsDuplicate(int languageId, string languageName);
        bool CreateLanguage(Language language);
        bool UpdateLanguage(Language language);
        bool DeleteLanguage(Language language);
        bool Save();
    }
}

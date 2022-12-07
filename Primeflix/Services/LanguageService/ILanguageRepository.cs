using Primeflix.Models;

namespace Primeflix.Services.LanguageService
{
    public interface ILanguageRepository
    {
        ICollection<Language> GetLanguages();
        Language GetLanguage(int languageId);
        Language GetLanguage(string languageCode);
        bool LanguageExists(int languageId);
        bool IsDuplicate(int languageId, string languageName);
        bool CreateLanguage(Language language);
        bool UpdateLanguage(Language language);
        bool DeleteLanguage(Language language);
        bool Save();
    }
}

using Primeflix.Models;

namespace Primeflix.Services.LanguageService
{
    public interface ILanguageRepository
    {
        Task<bool> LanguageExists(int languageId);
        Task<bool> LanguageExists(string languageCode);
        Task<bool> IsDuplicate(string languageName);
        Task<ICollection<Language>> GetLanguages();
        Task<Language> GetLanguage(int languageId);
        Task<Language> GetLanguage(string languageCode);
        Task<Language> GetLanguageOfAUser(int userId);
        Task<bool> CreateLanguage(Language language);
        Task<bool> UpdateLanguage(Language language);
        Task<bool> DeleteLanguage(Language language);
        Task<bool> Save();
    }
}

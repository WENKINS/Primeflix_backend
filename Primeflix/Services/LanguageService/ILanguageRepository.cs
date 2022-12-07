using Primeflix.Models;

namespace Primeflix.Services.LanguageService
{
    public interface ILanguageRepository
    {
        Task<ICollection<Language>> GetLanguages();
        Task<Language> GetLanguage(int languageId);
        Task<Language> GetLanguage(string languageCode);
        Task<bool> LanguageExists(int languageId);
        Task<bool> IsDuplicate(int languageId, string languageName);
        Task<bool> CreateLanguage(Language language);
        Task<bool> UpdateLanguage(Language language);
        Task<bool> DeleteLanguage(Language language);
        Task<bool> Save();
    }
}

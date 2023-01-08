using Primeflix.Models;

namespace Primeflix.Services.ProductTranslationService
{
    public interface IProductTranslationRepository
    {
        Task<bool> ProductTranslationExists(int productId, int languageId);
        Task<bool> IsDuplicate(int productId, int languageId);
        Task<ICollection<ProductTranslation>> GetProductsTranslations();
        Task<ProductTranslation> GetProductTranslation(int productId, string languageCode);
        Task<ICollection<ProductTranslation>> GetTranslationsOfAProduct(int productId);
        Task<ICollection<ProductTranslation>> GetProductsOfALanguage(int languageId);
        Task<bool> CreateProductTranslation(ProductTranslation productTranslation);
        Task<bool> UpdateProductTranslation(ProductTranslation productTranslation);
        Task<bool> DeleteProductTranslation(ProductTranslation productTranslation);
        Task<bool> Save();
    }
}

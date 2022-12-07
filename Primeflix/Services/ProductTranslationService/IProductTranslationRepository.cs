using Primeflix.Models;

namespace Primeflix.Services.ProductTranslationService
{
    public interface IProductTranslationRepository
    {
        ICollection<ProductTranslation> GetProductsTranslations();
        ProductTranslation GetProductTranslation(int productId, string languageCode);
        ICollection<ProductTranslation> GetTranslationsOfAProduct(int productId);
        ICollection<ProductTranslation> GetProductsOfALanguage(int languageId);
        bool ProductTranslationExists(int productId, int languageId);
        bool IsDuplicate(int productId, int languageId);
        bool CreateProductTranslation(ProductTranslation productTranslation);
        bool UpdateProductTranslation(ProductTranslation productTranslation);
        bool DeleteProductTranslation(ProductTranslation productTranslation);
        bool Save();
    }
}

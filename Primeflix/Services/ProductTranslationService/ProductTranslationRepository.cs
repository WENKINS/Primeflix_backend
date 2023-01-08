using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.ProductTranslationService
{
    public class ProductTranslationRepository : IProductTranslationRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProductTranslationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> ProductTranslationExists(int productId, int languageId)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId && pt.LanguageId == languageId).Any();
        }

        public async Task<bool> IsDuplicate(int productId, int languageId)
        {
            var productTranslation = _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId && pt.LanguageId == languageId).FirstOrDefault();
            return productTranslation == null ? false : true;
        }

        public async Task<ICollection<ProductTranslation>> GetProductsTranslations()
        {
            return _databaseContext.ProductsTranslations.ToList();
        }

        public async Task<ProductTranslation> GetProductTranslation(int productId, string languageCode)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId && pt.Language.Code == languageCode).FirstOrDefault();
        }

        public async Task<ICollection<ProductTranslation>> GetTranslationsOfAProduct(int productId)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId).ToList();
        }

        public async Task<ICollection<ProductTranslation>> GetProductsOfALanguage(int languageId)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.LanguageId == languageId).ToList();
        }

        public async Task<bool> CreateProductTranslation(ProductTranslation productTranslation)
        {
            _databaseContext.Add(productTranslation);
            return await Save();
        }

        public async Task<bool> UpdateProductTranslation(ProductTranslation productTranslation)
        {
            _databaseContext.Update(productTranslation);
            return await Save();
        }

        public async Task<bool> DeleteProductTranslation(ProductTranslation productTranslation)
        {
            _databaseContext.Remove(productTranslation);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}

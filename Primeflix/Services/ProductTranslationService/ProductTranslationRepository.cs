using Primeflix.Data;
using Primeflix.Models;

namespace Primeflix.Services.ProductTranslationService
{
    public class ProductTranslationRepository : IProductTranslationRepository
    {
        private DatabaseContext _databaseContext;

        public ProductTranslationRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public bool ProductTranslationExists(int productId, int languageId)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId && pt.LanguageId == languageId).Any();
        }

        public ICollection<ProductTranslation> GetProductsTranslations()
        {
            return _databaseContext.ProductsTranslations.ToList();
        }

        public ProductTranslation GetProductTranslation(int productId, string languageCode)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId && pt.Language.Code == languageCode).FirstOrDefault();
        }

        public bool IsDuplicate(int productId, int languageId)
        {
            var productTranslation = _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId && pt.LanguageId == languageId).FirstOrDefault();
            return productTranslation == null ? false : true;
        }

        public ICollection<ProductTranslation> GetTranslationsOfAProduct(int productId)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.ProductId == productId).ToList();
        }

        public ICollection<ProductTranslation> GetProductsOfALanguage(int languageId)
        {
            return _databaseContext.ProductsTranslations.Where(pt => pt.LanguageId == languageId).ToList();
        }

        public bool CreateProductTranslation(ProductTranslation productTranslation)
        {
            _databaseContext.Add(productTranslation);
            return Save();
        }

        public bool UpdateProductTranslation(ProductTranslation productTranslation)
        {
            _databaseContext.Update(productTranslation);
            return Save();
        }

        public bool DeleteProductTranslation(ProductTranslation productTranslation)
        {
            _databaseContext.Remove(productTranslation);
            return Save();
        }

        public bool Save()
        {
            return _databaseContext.SaveChanges() < 0 ? false : true;
        }
    }
}
